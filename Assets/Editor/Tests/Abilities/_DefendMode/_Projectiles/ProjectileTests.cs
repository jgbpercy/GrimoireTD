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
        private float startX = 1f;
        private float startY = 1f;
        private float startZ = 1f;
        private Vector3 startPosition;

        private float targetX = 2f;
        private float targetY = 3f;
        private float targetZ = 4f;
        private ICreep targetCreep;

        private IDefendingEntity sourceDefendingEntity;

        private float startSpeed = 5f;

        private IProjectileTemplate template;

        private IAttackEffect attackEffectOne;
        private IAttackEffect attackEffectTwo;
        private IEnumerable<IAttackEffect> attackEffects;

        private float deltaTime = 0.2f;

        private CProjectile subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            CDebug.InitialiseDebugChannels();

            startPosition = new Vector3(startX, startY, startZ);

            targetCreep = Substitute.For<ICreep>();

            targetCreep.TargetPosition().Returns(new Vector3(targetX, targetY, targetZ));

            sourceDefendingEntity = Substitute.For<IDefendingEntity>();

            template = Substitute.For<IProjectileTemplate>();

            template.Speed.Returns(startSpeed);

            attackEffectOne = Substitute.For<IAttackEffect>();
            attackEffectTwo = Substitute.For<IAttackEffect>();
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

            subject = new CProjectile(
                startPosition,
                targetCreep,
                template,
                sourceDefendingEntity
            );
        }

        [Test]
        public void ModelObjectFrameUpdate_ForNewProjectileWithTarget_MovesTowardsTargetAtTheCorrectRate()
        {
            subject.ModelObjectFrameUpdate(deltaTime);

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

            float distanceDeltaFactor = (deltaTime * startSpeed) / totalStartToTargetMagnitude;

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
            subject.HitCreep(targetCreep, 5f);

            targetCreep.Received(1).ApplyAttackEffects(attackEffects, sourceDefendingEntity);
        }

        [Test]
        public void HitCreep_FiresOnDestroyProjectileEvent()
        {
            float destructionDelay = 3f;

            var eventTester = new EventTester<EAOnDestroyProjectile>();
            subject.OnDestroyProjectile += eventTester.Handler;

            subject.HitCreep(targetCreep, destructionDelay);

            eventTester.AssertFired(true);
            eventTester.AssertResult(subject, x => Mathf.Approximately(x.WaitSeconds, destructionDelay));
        }

        [Test]
        public void ModelObjectFrameUpdate_AfterHittingCreep_DoesNotMoveProjectile()
        {
            subject.ModelObjectFrameUpdate(deltaTime);

            var positionAfterOneTick = subject.Position;

            subject.HitCreep(targetCreep, 5f);

            subject.ModelObjectFrameUpdate(deltaTime);

            Assert.True(CustomMath.Approximately(positionAfterOneTick, subject.Position));
        }

        [Test]
        public void ModelObjectFrameUpdate_WhenTargetDiesAndProjectileHasNoDirection_FiresOnDestroyEventWithNoDelay()
        {
            var eventTester = new EventTester<EAOnDestroyProjectile>();
            subject.OnDestroyProjectile += eventTester.Handler;

            targetCreep.OnDied += Raise.Event();

            subject.ModelObjectFrameUpdate(deltaTime);

            eventTester.AssertFired(true);
            eventTester.AssertResult(subject, x => Mathf.Approximately(x.WaitSeconds, 0f));
        }

        [Test]
        public void ModelObjectFrameUpdate_WhenTargetDiesAndProjectileAlreadyMoved_FiresOnDestroyEventWithStandardDelay()
        {
            var eventTester = new EventTester<EAOnDestroyProjectile>();
            subject.OnDestroyProjectile += eventTester.Handler;

            subject.ModelObjectFrameUpdate(deltaTime);

            targetCreep.OnDied += Raise.Event();

            subject.ModelObjectFrameUpdate(deltaTime);

            eventTester.AssertFired(true);
            eventTester.AssertResult(subject, x => Mathf.Approximately(x.WaitSeconds, CProjectile.NoTargetDestructionDelay));
        }

        [Test]
        public void ModelObjectFrameUpdate_WhenTargetDiesAndProjectileAlreadyMoved_ContinuesInPreviousDirection()
        {
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

            float distanceDeltaFactor = (2 * deltaTime * startSpeed) / totalStartToTargetMagnitude;

            Vector3 expectedPos = new Vector3(
                startX + startToTarget.x * distanceDeltaFactor,
                startY + startToTarget.y * distanceDeltaFactor,
                startZ + startToTarget.z * distanceDeltaFactor
            );

            subject.ModelObjectFrameUpdate(deltaTime);

            targetCreep.OnDied += Raise.Event();

            subject.ModelObjectFrameUpdate(deltaTime);

            Assert.True(CustomMath.Approximately(expectedPos, subject.Position));
        }
    }
}