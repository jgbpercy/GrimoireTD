using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.Abilities.DefendMode.Projectiles;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;
using UnityEngine;

namespace GrimoireTD.Tests.ProjectileLauncherEffectComponentTests
{
    public class ProjectileLauncherEffectComponentTests
    {
        private IProjectileLauncherComponentTemplate projectileLauncherComponentTemplate;

        private IDefendingEntity defendingEntity;

        private IProjectileTemplate projectileTemplate;

        private IProjectile projectile;

        private IDefendModeTargetable targetOne;
        private IDefendModeTargetable targetTwo;

        private List<IDefendModeTargetable> targetList;

        private Vector3 projectileCreationPosition;

        private CProjectileLauncherComponent subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var deCoord = new Coord(1, 1);

            defendingEntity = Substitute.For<IDefendingEntity>();

            defendingEntity.CoordPosition.Returns(deCoord);

            projectileCreationPosition = deCoord.ToFirePointVector();

            projectile = Substitute.For<IProjectile>();

            projectileTemplate = Substitute.For<IProjectileTemplate>();

            projectileTemplate.GenerateProjectile(
                Arg.Any<Vector3>(), 
                Arg.Any<IDefendModeTargetable>(), 
                Arg.Any<IDefendingEntity>()
            )
                .Returns(projectile);

            projectileLauncherComponentTemplate = Substitute.For<IProjectileLauncherComponentTemplate>();

            projectileLauncherComponentTemplate.ProjectileToFireTemplate.Returns(projectileTemplate);

            targetOne = Substitute.For<IDefendModeTargetable>();
            targetTwo = Substitute.For<IDefendModeTargetable>();

            targetList = new List<IDefendModeTargetable>
            {
                targetOne
            };

            subject = new CProjectileLauncherComponent(projectileLauncherComponentTemplate);
        }

        [SetUp]
        public void SetUp()
        {
            projectileTemplate.ClearReceivedCalls();

            defendingEntity.ClearReceivedCalls();
        }

        [Test]
        public void ExecuteEffect_PassedOneTarget_CreatesOneProjectile()
        {
            subject.ExecuteEffect(defendingEntity, targetList);

            projectileTemplate.Received(1).GenerateProjectile(
                Arg.Any<Vector3>(),
                Arg.Any<IDefendModeTargetable>(),
                Arg.Any<IDefendingEntity>()
            );
        }

        [Test]
        public void ExecuteEffect_PassedOneTarget_CreatesProjectileAtTheCorrectPosition()
        {
            subject.ExecuteEffect(defendingEntity, targetList);

            projectileTemplate.Received(1).GenerateProjectile(
                Arg.Is<Vector3>(vec => Mathf.Approximately(vec.x, projectileCreationPosition.x) && Mathf.Approximately(vec.y, projectileCreationPosition.y) && Mathf.Approximately(vec.z, projectileCreationPosition.z)),
                Arg.Any<IDefendModeTargetable>(),
                Arg.Any<IDefendingEntity>()
            );
        }

        [Test]
        public void ExecuteEffect_PassedOneTarget_CreatesProjectileWithCorrectTarget()
        {
            subject.ExecuteEffect(defendingEntity, targetList);

            projectileTemplate.Received(1).GenerateProjectile(
                Arg.Any<Vector3>(),
                targetOne,
                Arg.Any<IDefendingEntity>()
            );
        }

        [Test]
        public void ExecuteEffect_PassedOneTarget_CreatesProjectileWithCorrectSource()
        {
            subject.ExecuteEffect(defendingEntity, targetList);

            projectileTemplate.Received(1).GenerateProjectile(
                Arg.Any<Vector3>(),
                Arg.Any<IDefendModeTargetable>(),
                defendingEntity
            );
        }

        [Test]
        public void ExecuteEffect_PassedOneTarget_CallsSourceEntityCreatedProjectile()
        {
            subject.ExecuteEffect(defendingEntity, targetList);

            defendingEntity.Received(1).CreatedProjectile(projectile);
        }

        [Test]
        public void ExecuteEffect_PassedTwoTargets_CreatesTwoProjectiles()
        {
            var multiTargetList = new List<IDefendModeTargetable>
            {
                targetOne,
                targetTwo
            };

            subject.ExecuteEffect(defendingEntity, multiTargetList);

            projectileTemplate.Received(2).GenerateProjectile(
                Arg.Any<Vector3>(),
                Arg.Any<IDefendModeTargetable>(),
                Arg.Any<IDefendingEntity>()
            );

            defendingEntity.Received(2).CreatedProjectile(Arg.Any<IProjectile>());
        }
    }
}