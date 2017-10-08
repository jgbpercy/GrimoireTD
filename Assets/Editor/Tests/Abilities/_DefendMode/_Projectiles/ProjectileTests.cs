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

namespace GrimoireTD.Tests.ProjectileTests
{
    public class ProjectileTests
    {
        private float startSpeed = 5f;

        private float defaultDeltaTime = 0.2f;

        private float startX = 1f;
        private float startY = 1f;
        private float startZ = 1f;
        private Vector3 startPosition;

        private float targetX = 2f;
        private float targetY = 3f;
        private float targetZ = 4f;
        private ICreep targetCreep = Substitute.For<ICreep>();

        private IDefendingEntity sourceDefendingEntity = Substitute.For<IDefendingEntity>();

        private IProjectileTemplate template = Substitute.For<IProjectileTemplate>();

        private IAttackEffect attackEffectOne = Substitute.For<IAttackEffect>();
        private IAttackEffect attackEffectTwo = Substitute.For<IAttackEffect>();
        private IEnumerable<IAttackEffect> attackEffects;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            CDebug.InitialiseDebugChannels();

            startPosition = new Vector3(startX, startY, startZ);

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
            GameObject testGo = new GameObject();

            testGo.AddComponent<ModelObjectFrameUpdater>();
        }

        private CProjectile ConstructSubject()
        {
            return new CProjectile(
                startPosition,
                targetCreep,
                template,
                sourceDefendingEntity
            );
        }

        [Test]
        public void ModelObjectFrameUpdate_ForNewProjectileWithTarget_MovesTowardsTargetAtTheCorrectRate()
        {
            var subject = ConstructSubject();

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            Vector3 startToTarget = new Vector3(
                targetX - startX,
                targetY - startY,
                targetZ - startZ
            );

            float totalStartToTargetMagnitude = Mathf.Sqrt(
                Mathf.Pow(startToTarget.x, 2) +
                Mathf.Pow(startToTarget.y, 2) +
                Mathf.Pow(startToTarget.z, 2)
            );

            float distanceDeltaFactor = (defaultDeltaTime * startSpeed) / totalStartToTargetMagnitude;

            Vector3 expectedPos = new Vector3(
                startX + startToTarget.x * distanceDeltaFactor,
                startY + startToTarget.y * distanceDeltaFactor,
                startZ + startToTarget.z * distanceDeltaFactor
            );

            Assert.True(CustomMath.Approximately(expectedPos, subject.Position));
        }

        [Test]
        public void HitCreep_AppliesAttackEffectsToCreep()
        {
            var subject = ConstructSubject();

            subject.HitCreep(targetCreep, 5f);

            targetCreep.Received(1).ApplyAttackEffects(attackEffects, sourceDefendingEntity);
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
        public void ModelObjectFrameUpdate_AfterHittingCreep_DoesNotMoveProjectile()
        {
            var subject = ConstructSubject();

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            var positionAfterOneFrame = subject.Position;

            subject.HitCreep(targetCreep, 5f);

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            Assert.True(CustomMath.Approximately(positionAfterOneFrame, subject.Position));
        }

        [Test]
        public void ModelObjectFrameUpdate_WhenTargetDiesAndProjectileHasNoDirection_FiresOnDestroyEventWithNoDelay()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnDestroyProjectile>();
            subject.OnDestroyProjectile += eventTester.Handler;

            targetCreep.OnDied += Raise.Event();

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            eventTester.AssertFired(true);
            eventTester.AssertResult(subject, x => CustomMath.Approximately(x.WaitSeconds, 0f));
        }

        [Test]
        public void ModelObjectFrameUpdate_WhenTargetDiesAndProjectileAlreadyMoved_FiresOnDestroyEventWithStandardDelay()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnDestroyProjectile>();
            subject.OnDestroyProjectile += eventTester.Handler;

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            targetCreep.OnDied += Raise.Event();

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            eventTester.AssertFired(true);
            eventTester.AssertResult(subject, x => CustomMath.Approximately(x.WaitSeconds, CProjectile.NoTargetDestructionDelay));
        }

        [Test]
        public void ModelObjectFrameUpdate_WhenTargetDiesAndProjectileAlreadyMoved_ContinuesInPreviousDirection()
        {
            var subject = ConstructSubject();

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            targetCreep.OnDied += Raise.Event();

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            Vector3 startToTarget = new Vector3(
                targetX - startX,
                targetY - startY,
                targetZ - startZ
            );

            float totalStartToTargetMagnitude = Mathf.Sqrt(
                Mathf.Pow(startToTarget.x, 2) +
                Mathf.Pow(startToTarget.y, 2) +
                Mathf.Pow(startToTarget.z, 2)
            );

            float distanceDeltaFactor = (2 * defaultDeltaTime * startSpeed) / totalStartToTargetMagnitude;

            Vector3 expectedPos = new Vector3(
                startX + startToTarget.x * distanceDeltaFactor,
                startY + startToTarget.y * distanceDeltaFactor,
                startZ + startToTarget.z * distanceDeltaFactor
            );

            Assert.True(CustomMath.Approximately(expectedPos, subject.Position));
        }
    }
}