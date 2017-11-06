using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.Projectiles;
using GrimoireTD.Technical;
using GrimoireTD.Defenders;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.Creeps;
using GrimoireTD.Dependencies;

namespace GrimoireTD.Tests.AoeProjectileTests
{
    public class AoeProjectileTests
    {
        //Primitives and Basic Objects
        private float explosionTime = 0.8f;

        private float finalAoeRadius = 2f;

        private float defaultDeltaTime = 0.2f;

        private Vector3 startPosition = new Vector3();

        //Model and Frame Updater
        private FrameUpdaterStub frameUpdater;

        //Template Deps
        private IAoeProjectileTemplate template = Substitute.For<IAoeProjectileTemplate>();

        private IAttackEffect attackEffectOne = Substitute.For<IAttackEffect>();
        private IAttackEffect attackEffectTwo = Substitute.For<IAttackEffect>();
        private IEnumerable<IAttackEffect> aoeAttackEffects;

        //Other Deps Passed To Ctor
        private ICreep targetCreep = Substitute.For<ICreep>();

        private IDefender sourceDefender = Substitute.For<IDefender>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //Model and Frame Updater
            DepsProv.TheModelObjectFrameUpdater = () =>
            {
                return frameUpdater;
            };

            //Template Deps
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
            frameUpdater = new FrameUpdaterStub();

            targetCreep.ClearReceivedCalls();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(DepsProv).TypeInitializer.Invoke(null, null);
        }

        public CAoeProjectile ConstructSubject()
        {
            return new CAoeProjectile(
                startPosition,
                targetCreep,
                template,
                sourceDefender
            );
        }

        [Test]
        public void HitCreepInAoe_AppliesAttackEffectsToCreep()
        {
            var subject = ConstructSubject();

            subject.HitCreepInAoe(targetCreep);

            targetCreep.Received(1).ApplyAttackEffects(aoeAttackEffects, sourceDefender);
        }

        [Test]
        public void FrameUpdate_AfterProjectileHitsCreep_FiresExplosionStartedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnExplosionStarted>();
            subject.OnExplosionStarted += eventTester.Handler;

            subject.HitCreep(targetCreep, 5f);

            frameUpdater.RunUpdate(defaultDeltaTime);
            frameUpdater.RunUpdate(defaultDeltaTime);

            eventTester.AssertFired(1);
        }

        [Test]
        public void FrameUpdate_AfterProjectileHitsCreep_ExpandsExplosionRadiusAsCubeRouteOfExplosionProportionPassed()
        {
            var subject = ConstructSubject();

            subject.HitCreep(targetCreep, 5f);

            frameUpdater.RunUpdate(defaultDeltaTime);
            float expectedAoeRadius = Mathf.Pow(defaultDeltaTime / explosionTime, 1/3) * finalAoeRadius;
            Assert.True(CustomMath.Approximately(subject.CurrentAoeRadius, expectedAoeRadius));

            frameUpdater.RunUpdate(defaultDeltaTime);
            expectedAoeRadius = Mathf.Pow((defaultDeltaTime * 2) / explosionTime, 1/3) * finalAoeRadius;
            Assert.True(CustomMath.Approximately(subject.CurrentAoeRadius, expectedAoeRadius));
        }

        [Test]
        public void FrameUpdate_AfterProjectileHitsCreepAndExplosionTimeHasPasses_FiresExplosionFinishedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnExplosionFinished>();
            subject.OnExplosionFinished += eventTester.Handler;

            subject.HitCreep(targetCreep, 5f);

            frameUpdater.RunUpdate(defaultDeltaTime);

            frameUpdater.RunUpdate(explosionTime - defaultDeltaTime);

            eventTester.AssertFired(1);
        }
    }
}
