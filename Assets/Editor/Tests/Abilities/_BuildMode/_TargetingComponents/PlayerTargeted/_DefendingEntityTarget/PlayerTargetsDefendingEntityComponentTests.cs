using System;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Map;
using GrimoireTD.Defenders;

namespace GrimoireTD.Tests.PlayerTargetsDefenderComponentTests
{
    public class PlayerTargetsDefenderComponentTests
    {
        private Coord targetCoord = new Coord(2, 3);

        private IReadOnlyMapData mapData = Substitute.For<IReadOnlyMapData>();

        private IDefender sourceDefender = Substitute.For<IDefender>();

        private IDefender targetDefender = Substitute.For<IDefender>();

        private IPlayerTargetsDefenderComponentTemplate template = Substitute.For<IPlayerTargetsDefenderComponentTemplate>();

        private IBuildModeTargetable target = Substitute.For<IBuildModeTargetable>();

        private List<IBuildModeTargetable> targetList;

        private PlayerTargetsDefenderArgs playerTargetsDefenderArgs;

        private BuildModeAutoTargetedArgs buildModeAutoTargetedArgs;

        private CPlayerTargetsDefenderComponent subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            playerTargetsDefenderArgs = new PlayerTargetsDefenderArgs(
                sourceDefender,
                targetDefender,
                mapData
            );

            template.TargetingRule
                .GenerateArgs(sourceDefender, targetDefender)
                .Returns(playerTargetsDefenderArgs);

            PlayerTargetsDefenderRuleService.RunRule = (args) =>
            {
                if (args == playerTargetsDefenderArgs)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            };

            buildModeAutoTargetedArgs = new BuildModeAutoTargetedArgs(targetCoord);

            template.AoeRule
                .GenerateArgs(targetCoord)
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

            subject = new CPlayerTargetsDefenderComponent(template);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(PlayerTargetsDefenderRuleService).TypeInitializer.Invoke(null, null);
            typeof(BuildModeAutoTargetedRuleService).TypeInitializer.Invoke(null, null);
        }

        [Test]
        public void IsValidTarget_PassedNonDefender_ThrowsException()
        {
            Assert.Throws(typeof(ArgumentException), () =>
                subject.IsValidTarget(
                    sourceDefender,
                    targetCoord
                )
            );
        }

        [Test]
        public void IsValidTarget_PassedValidInput_ReturnsCorrectRuleResultForTheArgsGeneratedFromInput()
        {
            var result = subject.IsValidTarget(
                sourceDefender,
                targetDefender
            );

            Assert.True(result);
        }

        [Test]
        public void FindTargets_PassedValidInput_ReturnsRuleResultForTheArgsGeneratedFromThisInput()
        {
            var result = subject.FindTargets(
                targetCoord
            );

            Assert.AreEqual(result, targetList);
        }
    }
}