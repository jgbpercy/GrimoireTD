using System;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Map;
using GrimoireTD.DefendingEntities;
using GrimoireTD.DefendingEntities.Units;

namespace GrimoireTD.Tests.PlayerTargetsDefendingEntityComponentTests
{
    public class PlayerTargetsDefendingEntityComponentTests
    {
        private Coord targetCoord;

        private IReadOnlyMapData mapData;

        private IDefendingEntity sourceDefendingEntity;

        private IDefendingEntity targetDefendingEntity;

        private PlayerTargetsDefendingEntityArgs playerTargetsDefendingEntityArgs;

        private BuildModeAutoTargetedArgs buildModeAutoTargetedArgs;

        private IPlayerTargetsDefendingEntityComponentTemplate template;

        private IBuildModeTargetable target;

        private List<IBuildModeTargetable> targetList;

        private CPlayerTargetsDefendingEntityComponent subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            targetCoord = new Coord(2, 3);

            mapData = Substitute.For<IReadOnlyMapData>();

            template = Substitute.For<IPlayerTargetsDefendingEntityComponentTemplate>();

            sourceDefendingEntity = Substitute.For<IDefendingEntity>();

            targetDefendingEntity = Substitute.For<IDefendingEntity>();

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

            target = Substitute.For<IBuildModeTargetable>();

            targetList = new List<IBuildModeTargetable>
            {
                target
            };

            BuildModeAbilityAutoTargetedRuleService.RunRule = (args) =>
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
            typeof(BuildModeAbilityAutoTargetedRuleService).TypeInitializer.Invoke(null, null);
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