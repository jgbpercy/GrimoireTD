using System;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Map;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Tests.PlayerTargetsDefendingEntityComponentTests
{
    public class PlayerTargetsDefendingEntityComponentTests
    {
        private Coord targetCoord = new Coord(2, 3);

        private IReadOnlyMapData mapData = Substitute.For<IReadOnlyMapData>();

        private IDefendingEntity sourceDefendingEntity = Substitute.For<IDefendingEntity>();

        private IDefendingEntity targetDefendingEntity = Substitute.For<IDefendingEntity>();

        private IPlayerTargetsDefendingEntityComponentTemplate template = Substitute.For<IPlayerTargetsDefendingEntityComponentTemplate>();

        private IBuildModeTargetable target = Substitute.For<IBuildModeTargetable>();

        private List<IBuildModeTargetable> targetList;

        private PlayerTargetsDefendingEntityArgs playerTargetsDefendingEntityArgs;

        private BuildModeAutoTargetedArgs buildModeAutoTargetedArgs;

        private CPlayerTargetsDefendingEntityComponent subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            playerTargetsDefendingEntityArgs = new PlayerTargetsDefendingEntityArgs(
                sourceDefendingEntity,
                targetDefendingEntity,
                mapData
            );

            template.TargetingRule
                .GenerateArgs(sourceDefendingEntity, targetDefendingEntity, mapData)
                .Returns(playerTargetsDefendingEntityArgs);

            PlayerTargetsDefendingEntityRuleService.RunRule = (args) =>
            {
                if (args == playerTargetsDefendingEntityArgs)
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

            subject = new CPlayerTargetsDefendingEntityComponent(template);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(PlayerTargetsDefendingEntityRuleService).TypeInitializer.Invoke(null, null);
            typeof(BuildModeAutoTargetedRuleService).TypeInitializer.Invoke(null, null);
        }

        [Test]
        public void IsValidTarget_PassedNonDefendingEntity_ThrowsException()
        {
            Assert.Throws(typeof(ArgumentException), () =>
                subject.IsValidTarget(
                    sourceDefendingEntity,
                    targetCoord,
                    mapData
                )
            );
        }

        [Test]
        public void IsValidTarget_PassedValidInput_ReturnsCorrectRuleResultForTheArgsGeneratedFromInput()
        {
            var result = subject.IsValidTarget(
                sourceDefendingEntity,
                targetDefendingEntity,
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