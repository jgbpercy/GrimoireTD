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
        private Vector3 startPosition;

        private ICreep targetCreep;

        private IDefendingEntity sourceDefendingEntity;

        private IAoeProjectileTemplate template;

        private IAttackEffect attackEffectOne;
        private IAttackEffect attackEffectTwo;
        private IEnumerable<IAttackEffect> aoeAttackEffects;

        private float explosionTime = 0.8f;

        private float finalAoeRadius = 2f;

        private float deltaTime = 0.2f;

        private CAoeProjectile subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            CDebug.InitialiseDebugChannels();

            startPosition = new Vector3();

            targetCreep = Substitute.For<ICreep>();

            sourceDefendingEntity = Substitute.For<IDefendingEntity>();

            template = Substitute.For<IAoeProjectileTemplate>();

            attackEffectOne = Substitute.For<IAttackEffect>();
            attackEffectTwo = Substitute.For<IAttackEffect>();
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
            GameObject testGo = new GameObject();

            testGo.AddComponent<ModelObjectFrameUpdater>();

            subject = new CAoeProjectile(
                startPosition,
                targetCreep,
                template,
                sourceDefendingEntity
            );

            targetCreep.ClearReceivedCalls();
        }

        [Test]
        public void HitCreepInAoe_AppliesAttackEffectsToCreep()
        {
            subject.HitCreepInAoe(targetCreep);

            targetCreep.Received(1).ApplyAttackEffects(aoeAttackEffects, sourceDefendingEntity);
        }

        [Test]
        public void ModelObjectFrameUpdatee_AfterProjectileHitsCreep_FiresExplosionStartedEvent()
        {
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
            subject.HitCreep(targetCreep, 5f);

            subject.ModelObjectFrameUpdate(deltaTime);
            float expectedAoeRadius = Mathf.Pow(deltaTime / explosionTime, 1/3) * finalAoeRadius;
            Assert.True(Mathf.Approximately(subject.CurrentAoeRadius, expectedAoeRadius));

            subject.ModelObjectFrameUpdate(deltaTime);
            expectedAoeRadius = Mathf.Pow((deltaTime * 2) / explosionTime, 1/3) * finalAoeRadius;
            Assert.True(Mathf.Approximately(subject.CurrentAoeRadius, expectedAoeRadius));
        }

        [Test]
        public void ModelObjectFrameUpdatee_AfterProjectileHitsCreepAndExplosionTimeHasPasses_FiresExplosionFinishedEvent()
        {
            var eventTester = new EventTester<EAOnExplosionFinished>();
            subject.OnExplosionFinished += eventTester.Handler;

            subject.HitCreep(targetCreep, 5f);

            subject.ModelObjectFrameUpdate(deltaTime);

            subject.ModelObjectFrameUpdate(explosionTime - deltaTime);

            eventTester.AssertFired(1);
        }
    }
}
