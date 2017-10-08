using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.Projectiles;
using GrimoireTD.Technical;
using GrimoireTD.DefendingEntities;
using GrimoireTD.ChannelDebug;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.Creeps;

namespace GrimoireTD.Tests.AoeProjectileTests
{
    public class AoeProjectileTests
    {
        private float explosionTime = 0.8f;

        private float finalAoeRadius = 2f;

        private float deltaTime = 0.2f;

        private Vector3 startPosition = new Vector3();

        private ICreep targetCreep = Substitute.For<ICreep>();

        private IDefendingEntity sourceDefendingEntity = Substitute.For<IDefendingEntity>();

        private IAoeProjectileTemplate template = Substitute.For<IAoeProjectileTemplate>();

        private IAttackEffect attackEffectOne = Substitute.For<IAttackEffect>();
        private IAttackEffect attackEffectTwo = Substitute.For<IAttackEffect>();
        private IEnumerable<IAttackEffect> aoeAttackEffects;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            CDebug.InitialiseDebugChannels();

            GameObject testGo = new GameObject();

            testGo.AddComponent<ModelObjectFrameUpdater>();

            aoeAttackEffects = new List<IAttackEffect>
            {
                attackEffectOne,
                attackEffectTwo
            };

            template.AoeAttackEffects.Returns(aoeAttackEffects);

            template.AoeExplosionTime.Returns(explosionTime);

            template.AoeRadius.Returns(finalAoeRadius);
        }

        [SetUp]
        public void EachTestSetUp()
        {
            targetCreep.ClearReceivedCalls();
        }

        public CAoeProjectile ConstructSubject()
        {
            return new CAoeProjectile(
                startPosition,
                targetCreep,
                template,
                sourceDefendingEntity
            );
        }

        [Test]
        public void HitCreepInAoe_AppliesAttackEffectsToCreep()
        {
            var subject = ConstructSubject();

            subject.HitCreepInAoe(targetCreep);

            targetCreep.Received(1).ApplyAttackEffects(aoeAttackEffects, sourceDefendingEntity);
        }

        [Test]
        public void ModelObjectFrameUpdatee_AfterProjectileHitsCreep_FiresExplosionStartedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnExplosionStarted>();
            subject.OnExplosionStarted += eventTester.Handler;

            subject.HitCreep(targetCreep, 5f);

            subject.ModelObjectFrameUpdate(deltaTime);
            subject.ModelObjectFrameUpdate(deltaTime);

            eventTester.AssertFired(1);
        }

        [Test]
        public void ModelObjectFrameUpdatee_AfterProjectileHitsCreep_ExpandsExplosionRadiusAsCubeRouteOfExplosionProportionPassed()
        {
            var subject = ConstructSubject();

            subject.HitCreep(targetCreep, 5f);

            subject.ModelObjectFrameUpdate(deltaTime);
            float expectedAoeRadius = Mathf.Pow(deltaTime / explosionTime, 1/3) * finalAoeRadius;
            Assert.True(CustomMath.Approximately(subject.CurrentAoeRadius, expectedAoeRadius));

            subject.ModelObjectFrameUpdate(deltaTime);
            expectedAoeRadius = Mathf.Pow((deltaTime * 2) / explosionTime, 1/3) * finalAoeRadius;
            Assert.True(CustomMath.Approximately(subject.CurrentAoeRadius, expectedAoeRadius));
        }

        [Test]
        public void ModelObjectFrameUpdatee_AfterProjectileHitsCreepAndExplosionTimeHasPasses_FiresExplosionFinishedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnExplosionFinished>();
            subject.OnExplosionFinished += eventTester.Handler;

            subject.HitCreep(targetCreep, 5f);

            subject.ModelObjectFrameUpdate(deltaTime);

            subject.ModelObjectFrameUpdate(explosionTime - deltaTime);

            eventTester.AssertFired(1);
        }
    }
}
