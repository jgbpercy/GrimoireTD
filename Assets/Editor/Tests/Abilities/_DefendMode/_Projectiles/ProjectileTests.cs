﻿using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.Projectiles;
using GrimoireTD.Technical;
using GrimoireTD.Defenders;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.Creeps;
using GrimoireTD.Dependencies;

namespace GrimoireTD.Tests.ProjectileTests
{
    public class ProjectileTests
    {
        //Primitives and Basic Objects
        private float startSpeed = 5f;

        private float defaultDeltaTime = 0.2f;

        private float startX = 1f;
        private float startY = 1f;
        private float startZ = 1f;
        private Vector3 startPosition;

        private float targetX = 2f;
        private float targetY = 3f;
        private float targetZ = 4f;

        //Model and Frame Updater
        private FrameUpdaterStub frameUpdater;

        //Template Deps
        private IProjectileTemplate template = Substitute.For<IProjectileTemplate>();

        private IAttackEffect attackEffectOne = Substitute.For<IAttackEffect>();
        private IAttackEffect attackEffectTwo = Substitute.For<IAttackEffect>();
        private IEnumerable<IAttackEffect> attackEffects;

        //Other Deps Passed To Ctor
        private ICreep targetCreep = Substitute.For<ICreep>();

        private IDefender sourceDefender = Substitute.For<IDefender>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            startPosition = new Vector3(startX, startY, startZ);

            //Model and Frame Updater
            DepsProv.TheModelObjectFrameUpdater = () =>
            {
                return frameUpdater;
            };

            //Template Deps
            targetCreep.TargetPosition().Returns(new Vector3(targetX, targetY, targetZ));

            template.Speed.Returns(startSpeed);

            attackEffects = new List<IAttackEffect>
            {
                attackEffectOne,
                attackEffectTwo
            };

            template.AttackEffects.Returns(attackEffects);
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

        private CProjectile ConstructSubject()
        {
            return new CProjectile(
                startPosition,
                targetCreep,
                template,
                sourceDefender
            );
        }

        [Test]
        public void FrameUpdate_ForNewProjectileWithTarget_MovesTowardsTargetAtTheCorrectRate()
        {
            var subject = ConstructSubject();

            frameUpdater.RunUpdate(defaultDeltaTime);

            var startToTargetVector = new Vector3(
                targetX - startX,
                targetY - startY,
                targetZ - startZ
            );

            float startToTargetDistance = Mathf.Sqrt(
                Mathf.Pow(startToTargetVector.x, 2) +
                Mathf.Pow(startToTargetVector.y, 2) +
                Mathf.Pow(startToTargetVector.z, 2)
            );

            float expectedProportionOfTotalDistanceTravelled = (defaultDeltaTime * startSpeed) / startToTargetDistance;

            var expectedPosition = new Vector3(
                startX + startToTargetVector.x * expectedProportionOfTotalDistanceTravelled,
                startY + startToTargetVector.y * expectedProportionOfTotalDistanceTravelled,
                startZ + startToTargetVector.z * expectedProportionOfTotalDistanceTravelled
            );

            Assert.True(CustomMath.Approximately(expectedPosition, subject.Position));
        }

        [Test]
        public void HitCreep_AppliesAttackEffectsToCreep()
        {
            var subject = ConstructSubject();

            subject.HitCreep(targetCreep, 5f);

            targetCreep.Received(1).ApplyAttackEffects(attackEffects, sourceDefender);
        }

        [Test]
        public void HitCreep_FiresOnDestroyProjectileEvent()
        {
            float destructionDelay = 3f;

            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnDestroyProjectile>();
            subject.OnDestroyProjectile += eventTester.Handler;

            subject.HitCreep(targetCreep, destructionDelay);

            eventTester.AssertFired(true);
            eventTester.AssertResult(subject, x => CustomMath.Approximately(x.WaitSeconds, destructionDelay));
        }

        [Test]
        public void FrameUpdate_AfterHittingCreep_DoesNotMoveProjectile()
        {
            var subject = ConstructSubject();

            frameUpdater.RunUpdate(defaultDeltaTime);

            var positionAfterOneFrame = subject.Position;

            subject.HitCreep(targetCreep, 5f);

            frameUpdater.RunUpdate(defaultDeltaTime);

            Assert.True(CustomMath.Approximately(positionAfterOneFrame, subject.Position));
        }

        [Test]
        public void FrameUpdate_WhenTargetDiesAndProjectileHasNoDirection_FiresOnDestroyEventWithNoDelay()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnDestroyProjectile>();
            subject.OnDestroyProjectile += eventTester.Handler;

            targetCreep.OnDied += Raise.Event();

            frameUpdater.RunUpdate(defaultDeltaTime);

            eventTester.AssertFired(true);
            eventTester.AssertResult(subject, x => CustomMath.Approximately(x.WaitSeconds, 0f));
        }

        [Test]
        public void FrameUpdate_WhenTargetDiesAndProjectileAlreadyMoved_FiresOnDestroyEventWithStandardDelay()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnDestroyProjectile>();
            subject.OnDestroyProjectile += eventTester.Handler;

            frameUpdater.RunUpdate(defaultDeltaTime);

            targetCreep.OnDied += Raise.Event();

            frameUpdater.RunUpdate(defaultDeltaTime);

            eventTester.AssertFired(true);
            eventTester.AssertResult(subject, x => CustomMath.Approximately(x.WaitSeconds, CProjectile.NoTargetDestructionDelay));
        }

        [Test]
        public void FrameUpdate_WhenTargetDiesAndProjectileAlreadyMoved_ContinuesInPreviousDirection()
        {
            var subject = ConstructSubject();

            frameUpdater.RunUpdate(defaultDeltaTime);

            targetCreep.OnDied += Raise.Event();

            frameUpdater.RunUpdate(defaultDeltaTime);

            var startToTargetVector = new Vector3(
                targetX - startX,
                targetY - startY,
                targetZ - startZ
            );

            float startToTargetDistance = Mathf.Sqrt(
                Mathf.Pow(startToTargetVector.x, 2) +
                Mathf.Pow(startToTargetVector.y, 2) +
                Mathf.Pow(startToTargetVector.z, 2)
            );

            float expectedProportionOfTotalDistanceTravelled = (2 * defaultDeltaTime * startSpeed) / startToTargetDistance;

            var expectedPosition = new Vector3(
                startX + startToTargetVector.x * expectedProportionOfTotalDistanceTravelled,
                startY + startToTargetVector.y * expectedProportionOfTotalDistanceTravelled,
                startZ + startToTargetVector.z * expectedProportionOfTotalDistanceTravelled
            );

            Assert.True(CustomMath.Approximately(expectedPosition, subject.Position));
        }
    }
}