using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.Abilities.DefendMode.Projectiles;
using GrimoireTD.Defenders;
using GrimoireTD.Map;
using GrimoireTD.Technical;

namespace GrimoireTD.Tests.ProjectileLauncherEffectComponentTests
{
    public class ProjectileLauncherEffectComponentTests
    {
        private Coord defenderCoord = new Coord(1, 1);

        private IProjectileLauncherComponentTemplate projectileLauncherComponentTemplate 
            = Substitute.For<IProjectileLauncherComponentTemplate>();

        private IDefender defender = Substitute.For<IDefender>();

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
            defender.CoordPosition.Returns(defenderCoord);

            projectileCreationPosition = defenderCoord.ToFirePointVector();

            projectileTemplate.GenerateProjectile(
                Arg.Any<Vector3>(), 
                Arg.Any<IDefendModeTargetable>(), 
                Arg.Any<IDefender>()
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

            defender.ClearReceivedCalls();
        }

        [Test]
        public void ExecuteEffect_PassedOneTarget_CreatesOneProjectile()
        {
            subject.ExecuteEffect(defender, targetList);

            projectileTemplate.Received(1).GenerateProjectile(
                Arg.Any<Vector3>(),
                Arg.Any<IDefendModeTargetable>(),
                Arg.Any<IDefender>()
            );
        }

        [Test]
        public void ExecuteEffect_PassedOneTarget_CreatesProjectileAtTheCorrectPosition()
        {
            subject.ExecuteEffect(defender, targetList);

            projectileTemplate.Received(1).GenerateProjectile(
                Arg.Is<Vector3>(vec => CustomMath.Approximately(vec, projectileCreationPosition)),
                Arg.Any<IDefendModeTargetable>(),
                Arg.Any<IDefender>()
            );
        }

        [Test]
        public void ExecuteEffect_PassedOneTarget_CreatesProjectileWithCorrectTarget()
        {
            subject.ExecuteEffect(defender, targetList);

            projectileTemplate.Received(1).GenerateProjectile(
                Arg.Any<Vector3>(),
                targetOne,
                Arg.Any<IDefender>()
            );
        }

        [Test]
        public void ExecuteEffect_PassedOneTarget_CreatesProjectileWithCorrectSource()
        {
            subject.ExecuteEffect(defender, targetList);

            projectileTemplate.Received(1).GenerateProjectile(
                Arg.Any<Vector3>(),
                Arg.Any<IDefendModeTargetable>(),
                defender
            );
        }

        [Test]
        public void ExecuteEffect_PassedOneTarget_CallsSourceDefenderCreatedProjectile()
        {
            subject.ExecuteEffect(defender, targetList);

            defender.Received(1).CreatedProjectile(projectile);
        }

        [Test]
        public void ExecuteEffect_PassedTwoTargets_CreatesTwoProjectiles()
        {
            var multiTargetList = new List<IDefendModeTargetable>
            {
                targetOne,
                targetTwo
            };

            subject.ExecuteEffect(defender, multiTargetList);

            projectileTemplate.Received(2).GenerateProjectile(
                Arg.Any<Vector3>(),
                Arg.Any<IDefendModeTargetable>(),
                Arg.Any<IDefender>()
            );

            defender.Received(2).CreatedProjectile(Arg.Any<IProjectile>());
        }
    }
}