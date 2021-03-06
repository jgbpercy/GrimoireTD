﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Map;
using GrimoireTD.Defenders;
using GrimoireTD.Defenders.Units;

namespace GrimoireTD.Tests.PlayerTargetsHexComponentTests
{
    public class PlayerTargetsHexComponentTests
    {
        private Coord targetCoord = new Coord(2, 3);

        private IUnit targetUnit = Substitute.For<IUnit>();

        private IDefender sourceDefender = Substitute.For<IDefender>();

        private IPlayerTargetsHexComponentTemplate template = Substitute.For<IPlayerTargetsHexComponentTemplate>();

        private IBuildModeTargetable target = Substitute.For<IBuildModeTargetable>();
        
        private PlayerTargetsHexArgs playerTargetsHexArgs;

        private BuildModeAutoTargetedArgs buildModeAutoTargetedArgs;

        private List<IBuildModeTargetable> targetList;

        private CPlayerTargetsHexComponent subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            playerTargetsHexArgs = new PlayerTargetsHexArgs(
                sourceDefender,
                targetCoord
            );

            template.TargetingRule
                .GenerateArgs(sourceDefender, targetCoord)
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
                    sourceDefender, 
                    targetUnit 
                )
            );
        }

        [Test]
        public void IsValidTarget_PassedValidInput_ReturnsCorrectRuleResultForTheArgsGeneratedFromInput()
        {
            var result = subject.IsValidTarget(
                sourceDefender,
                targetCoord
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