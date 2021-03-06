﻿using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Defenders.Units;
using GrimoireTD.Map;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Defenders;

namespace GrimoireTD.Tests.BuildModeAbilityTests
{
    public class BuildModeAbilityTests
    {
        private IUnit unit = Substitute.For<IUnit>();

        private IBuildModeTargetingComponent targetingComponent = Substitute.For<IBuildModeTargetingComponent>();

        private IReadOnlyList<IBuildModeTargetable> returnedTargetList;

        private IBuildModeEffectComponentTemplate effectComponentTemplate = Substitute.For<IBuildModeEffectComponentTemplate>();
        private IBuildModeEffectComponentTemplate effectComponentTemplateTwo = Substitute.For<IBuildModeEffectComponentTemplate>();

        private IBuildModeEffectComponent effectComponent = Substitute.For<IBuildModeEffectComponent>();
        private IBuildModeEffectComponent effectComponentTwo = Substitute.For<IBuildModeEffectComponent>();

        private IBuildModeAbilityTemplate template = Substitute.For<IBuildModeAbilityTemplate>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            returnedTargetList = new List<IBuildModeTargetable>
            {
                new Coord(1, 1)
            };

            targetingComponent
                .FindTargets(Arg.Any<Coord>())
                .Returns(returnedTargetList);

            effectComponentTemplate.GenerateEffectComponent().Returns(effectComponent);
            effectComponentTemplateTwo.GenerateEffectComponent().Returns(effectComponentTwo);

            template.TargetingComponentTemplate.GenerateTargetingComponent().Returns(targetingComponent);
            template.EffectComponentTemplates.Returns(new List<IBuildModeEffectComponentTemplate>
            {
                effectComponentTemplate,
                effectComponentTemplateTwo
            });
        }

        [SetUp]
        public void EachTestSetUp()
        {
            effectComponent.ClearReceivedCalls();
            effectComponentTwo.ClearReceivedCalls();
        }

        private CBuildModeAbility ConstructSubject()
        {
            return new  CBuildModeAbility(template);
        }

        [Test]
        public void ExecuteAbility_PassedExecutingDefender_ExecutesEffectWithPassedDefender()
        {
            var subject = ConstructSubject();

            subject.ExecuteAbility(unit, new Coord(0, 0));

            effectComponent.Received(1).ExecuteEffect(unit, Arg.Any<IReadOnlyList<IBuildModeTargetable>>());
            effectComponentTwo.Received(1).ExecuteEffect(unit, Arg.Any<IReadOnlyList<IBuildModeTargetable>>());
        }

        [Test]
	    public void ExecuteAbility_PassedValidInput_ExecutesEffectWithReturnedTargets()
        {
            var subject = ConstructSubject();

            subject.ExecuteAbility(unit, new Coord(0, 0));

            effectComponent.Received(1).ExecuteEffect(Arg.Any<IDefender>(), returnedTargetList);
            effectComponentTwo.Received(1).ExecuteEffect(Arg.Any<IDefender>(), returnedTargetList);
        }

        [Test]
        public void ExecuteAbility_PassedValidInput_FiresOnExecutedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnExecutedBuildModeAbility>();
            subject.OnExecuted += eventTester.Handler;

            subject.ExecuteAbility(unit, new Coord(0, 0));

            eventTester.AssertFired(1);
            Assert.AreEqual(eventTester.SenderResult, subject);
            Assert.AreEqual(eventTester.ArgsResult.ExecutedAbility, subject);
        }
    }
}
