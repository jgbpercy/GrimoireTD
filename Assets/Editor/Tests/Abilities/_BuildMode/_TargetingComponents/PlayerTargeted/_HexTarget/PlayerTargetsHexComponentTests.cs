using System;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Map;
using GrimoireTD.DefendingEntities;
using GrimoireTD.DefendingEntities.Units;

namespace GrimoireTD.Tests.PlayerTargetsHexComponentTests
{
    public class PlayerTargetsHexComponentTests
    {
        private Coord targetCoord;

        private IUnit targetUnit;

        private IReadOnlyMapData mapData;

        private IDefendingEntity sourceDefendingEntity;

        private PlayerTargetsHexArgs playerTargetsHexArgs;

        private BuildModeAutoTargetedArgs buildModeAutoTargetedArgs;

        private IPlayerTargetsHexComponentTemplate template;

        private IBuildModeTargetable target;

        private List<IBuildModeTargetable> targetList;

        private CPlayerTargetsHexComponent subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            targetCoord = new Coord(2, 3);

            mapData = Substitute.For<IReadOnlyMapData>();

            template = Substitute.For<IPlayerTargetsHexComponentTemplate>();

            sourceDefendingEntity = Substitute.For<IDefendingEntity>();

            targetUnit = Substitute.For<IUnit>();

            playerTargetsHexArgs = new PlayerTargetsHexArgs(
                sourceDefendingEntity,
                targetCoord,
                mapData
            );

            template.TargetingRule
                .GenerateArgs(sourceDefendingEntity, targetCoord, mapData)
                .Returns(playerTargetsHexArgs);

            PlayerTargetsHexRuleService.RunRule = (args) =>
            {
                if (args == playerTargetsHexArgs)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            };

            buildModeAutoTargetedArgs = new BuildModeAutoTargetedArgs(
                targetCoord,
                mapData
            );

            template.AoeRule
                .GenerateArgs(targetCoord, mapData)
                .Returns(buildModeAutoTargetedArgs);

            target = Substitute.For<IBuildModeTargetable>();

            targetList = new List<IBuildModeTargetable>
            {
                target
            };

            BuildModeAutoTargetedRuleService.RunRule = (args) =>
            {
                if (args == buildModeAutoTargetedArgs)
                {
                    return targetList;
                }
                else
                {
                    return null;
                }
            };

            subject = new CPlayerTargetsHexComponent(template);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(PlayerTargetsHexRuleService).TypeInitializer.Invoke(null, null);
            typeof(BuildModeAutoTargetedRuleService).TypeInitializer.Invoke(null, null);
        }

        [Test]
        public void IsValidTarget_PassedNonCoordinate_ThrowsException()
        {
            Assert.Throws(typeof(ArgumentException), () =>
                subject.IsValidTarget(
                    sourceDefendingEntity, 
                    targetUnit, 
                    mapData
                )
            );
        }

        [Test]
        public void IsValidTarget_PassedValidInput_ReturnsCorrectRuleResultForTheArgsGeneratedFromInput()
        {
            var result = subject.IsValidTarget(
                sourceDefendingEntity,
                targetCoord,
                mapData
            );

            Assert.True(result);
        }

        [Test]
        public void FindTargets_PassedValidInput_ReturnsRuleResultForTheArgsGeneratedFromThisInput()
        {
            var result = subject.FindTargets(
                targetCoord,
                mapData
            );

            Assert.AreEqual(result, targetList);
        }
    }
}