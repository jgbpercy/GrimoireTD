using System;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.DefendingEntities.Units;
using GrimoireTD.Map;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Tests.BuildModeAbilityTests
{
    public class BuildModeAbilityTests
    {
        private IUnit unit;

        private IBuildModeTargetingComponent targetingComponent;

        private Coord executionPosition;

        private IReadOnlyList<IBuildModeTargetable> returnedTargetList;

        private IReadOnlyMapData mapData;

        private IBuildModeEffectComponentTemplate effectComponentTemplate;

        private IBuildModeEffectComponentTemplate effectComponentTemplateTwo;

        private IBuildModeEffectComponent effectComponent;

        private IBuildModeEffectComponent effectComponentTwo;

        private IBuildModeAbilityTemplate template;

        private CBuildModeAbility subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            unit = Substitute.For<IUnit>();

            targetingComponent = Substitute.For<IBuildModeTargetingComponent>();

            returnedTargetList = new List<IBuildModeTargetable>
            {
                new Coord(1, 1)
            };

            mapData = Substitute.For<IReadOnlyMapData>();

            targetingComponent
                .FindTargets(Arg.Any<Coord>(), mapData)
                .Returns(returnedTargetList);

            effectComponentTemplate = Substitute.For<IBuildModeEffectComponentTemplate>();
            effectComponentTemplateTwo = Substitute.For<IBuildModeEffectComponentTemplate>();

            effectComponent = Substitute.For<IBuildModeEffectComponent>();
            effectComponentTwo = Substitute.For<IBuildModeEffectComponent>();

            effectComponentTemplate.GenerateEffectComponent().Returns(effectComponent);
            effectComponentTemplateTwo.GenerateEffectComponent().Returns(effectComponentTwo);

            template = Substitute.For<IBuildModeAbilityTemplate>();
            template.TargetingComponent.Returns(targetingComponent);
            template.EffectComponentTemplates.Returns(new List<IBuildModeEffectComponentTemplate> { effectComponentTemplate, effectComponentTemplateTwo });

            subject = new CBuildModeAbility(template);
        }

        [Test]
        public void ExecuteAbility_PassedExecutionPosition_PassesExecutionPositionToTargetingComponent()
        {
            var executionPosition = new Coord(3, 3);

            subject.ExecuteAbility(unit, executionPosition, mapData);

            targetingComponent.Received().FindTargets(Arg.Is(executionPosition), Arg.Any<IReadOnlyMapData>());
        }

        [Test]
        public void ExecuteAbility_PassedExecutingEntity_PassesExecutingEntityToEffectComponents()
        {
            subject.ExecuteAbility(unit, new Coord(0, 0), mapData);

            effectComponent.Received().ExecuteEffect(Arg.Is(unit), Arg.Any<IReadOnlyList<IBuildModeTargetable>>());
            effectComponentTwo.Received().ExecuteEffect(Arg.Is(unit), Arg.Any<IReadOnlyList<IBuildModeTargetable>>());
        }

        [Test]
	    public void ExecuteAbility_PassedValidInput_PassesTargetsToEffectComponents()
        {
            subject.ExecuteAbility(unit, new Coord(0, 0), mapData);

            effectComponent.Received().ExecuteEffect(Arg.Any<IDefendingEntity>(), Arg.Is(returnedTargetList));
            effectComponentTwo.Received().ExecuteEffect(Arg.Any<IDefendingEntity>(), Arg.Is(returnedTargetList));
        }

        [Test]
        public void ExecuteAbility_PassedValidInput_FiresOnExecutedEvent()
        {
            var eventFired = false;
            object testSender = null;
            EAOnExecutedBuildModeAbility testArgs = null;

            var eventHandler = new EventHandler<EAOnExecutedBuildModeAbility>((object sender, EAOnExecutedBuildModeAbility args) =>
            {
                eventFired = true;
                testSender = sender;
                testArgs = args;
            });

            subject.OnExecuted += eventHandler;

            subject.ExecuteAbility(unit, new Coord(0, 0), mapData);

            Assert.True(eventFired);
            Assert.AreEqual(testSender, subject);
            Assert.AreEqual(testArgs.ExecutedAbility, subject);
        }
    }
}
