using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.Abilities.DefendMode.Projectiles;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;
using GrimoireTD.Technical;

namespace GrimoireTD.Tests.ProjectileLauncherEffectComponentTests
{
    public class ProjectileLauncherEffectComponentTests
    {
        private Coord defendingEntityCoord = new Coord(1, 1);

        private IProjectileLauncherComponentTemplate projectileLauncherComponentTemplate 
            = Substitute.For<IProjectileLauncherComponentTemplate>();

        private IDefendingEntity defendingEntity = Substitute.For<IDefendingEntity>();

        private IProjectileTemplate projectileTemplate = Substitute.For<IProjectileTemplate>();

        private IProjectile projectile = Substitute.For<IProjectile>();

        private IDefendModeTargetable targetOne = Substitute.For<IDefendModeTargetable>();
        private IDefendModeTargetable targetTwo = Substitute.For<IDefendModeTargetable>();

        private List<IDefendModeTargetable> targetList;

        private Vector3 projectileCreationPosition;

        private CProjectileLauncherComponent subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            defendingEntity.CoordPosition.Returns(defendingEntityCoord);

            projectileCreationPosition = defendingEntityCoord.ToFirePointVector();

            projectileTemplate.GenerateProjectile(
                Arg.Any<Vector3>(), 
                Arg.Any<IDefendModeTargetable>(), 
                Arg.Any<IDefendingEntity>()
            )
                .Returns(projectile);

            projectileLauncherComponentTemplate.ProjectileToFireTemplate.Returns(projectileTemplate);

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
                Arg.Is<Vector3>(vec => CustomMath.Approximately(vec, projectileCreationPosition)),
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