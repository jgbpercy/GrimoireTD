﻿using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.Defenders;
using GrimoireTD.Attributes;
using GrimoireTD.Creeps;
using GrimoireTD.Abilities;
using GrimoireTD.Dependencies;

namespace GrimoireTD.Tests.DefendModeAbilityTests
{
    public class DefendModeAbilityTests
    {
        //Primitives and Basic Objects
        private float defaultBaseCooldown = 5f;

        private float defaultDeltaTime = 0.2f;

        //Model and Frame Updater
        private FrameUpdaterStub frameUpdater;

        private IReadOnlyGameModel gameModel = Substitute.For<IReadOnlyGameModel>();

        private IReadOnlyGameModeManager gameModeManager = Substitute.For<IReadOnlyGameModeManager>();

        private IReadOnlyCreepManager creepManager = Substitute.For<IReadOnlyCreepManager>();

        //Template Deps
        private IDefendModeAbilityTemplate template = Substitute.For<IDefendModeAbilityTemplate>();

        private IDefendModeTargetingComponent targetingComponent = Substitute.For<IDefendModeTargetingComponent>();

        private IDefendModeEffectComponentTemplate effectComponentTemplateOne = Substitute.For<IDefendModeEffectComponentTemplate>();
        private IDefendModeEffectComponentTemplate effectComponentTemplateTwo = Substitute.For<IDefendModeEffectComponentTemplate>();

        private IDefendModeEffectComponent effectComponentOne = Substitute.For<IDefendModeEffectComponent>();
        private IDefendModeEffectComponent effectComponentTwo = Substitute.For<IDefendModeEffectComponent>();

        //Other Deps Passed To Ctor
        private IDefender attachedToDefender = Substitute.For<IDefender>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //Model and Frame Updater
            gameModel.GameModeManager.Returns(gameModeManager);

            gameModel.CreepManager.Returns(creepManager);

            DepsProv.SetTheGameModel(gameModel);

            DepsProv.TheModelObjectFrameUpdater = () =>
            {
                return frameUpdater;
            };

            //Template Deps
            template.TargetingComponentTemplate.GenerateTargetingComponent().Returns(targetingComponent);

            effectComponentTemplateOne.GenerateEffectComponent().Returns(effectComponentOne);
            effectComponentTemplateTwo.GenerateEffectComponent().Returns(effectComponentTwo);

            template.EffectComponentTemplates.Returns(new List<IDefendModeEffectComponentTemplate>
            {
                effectComponentTemplateOne,
                effectComponentTemplateTwo
            });
        }

        [SetUp]
        public void EachTestSetUp()
        {
            frameUpdater = new FrameUpdaterStub();

            template.BaseCooldown.Returns(defaultBaseCooldown);

            gameModeManager.CurrentGameMode.Returns(GameMode.DEFEND);

            attachedToDefender.Attributes.Get(DeAttrName.cooldownReduction).Value().Returns(0f);

            effectComponentOne.ClearReceivedCalls();
            effectComponentTwo.ClearReceivedCalls();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(DepsProv).TypeInitializer.Invoke(null, null);
        }

        private CDefendModeAbility ConstructSubject()
        {
            return new CDefendModeAbility(
                template,
                attachedToDefender
            );
        }

        [Test]
        public void ActualCooldown_WhenDefenderHasNoCooldownReduction_IsBaseCooldown()
        {
            attachedToDefender.Attributes.Get(DeAttrName.cooldownReduction).Value().Returns(0f);

            var subject = ConstructSubject();

            AssertExt.Approximately(defaultBaseCooldown, subject.ActualCooldown);
        }

        [Test]
        public void ActualCooldown_WhenDefenderHasCooldownReduction_TakesReductionIntoAcount()
        {
            attachedToDefender.Attributes.Get(DeAttrName.cooldownReduction).Value().Returns(0.2f);

            var subject = ConstructSubject();

            AssertExt.Approximately(defaultBaseCooldown * (1 - 0.2f), subject.ActualCooldown);
        }

        [Test]
        public void ActualCooldown_WhenDefenderCooldownReductionChanges_TakesReductionIntoAccount()
        {
            attachedToDefender.Attributes.Get(DeAttrName.cooldownReduction).Value().Returns(0f);

            var subject = ConstructSubject();

            attachedToDefender.Attributes.Get(DeAttrName.cooldownReduction).Value().Returns(0.3f);

            attachedToDefender.Attributes.Get(DeAttrName.cooldownReduction)
                .OnAttributeChanged += Raise.EventWith(new EAOnAttributeChanged(0.3f));

            AssertExt.Approximately(defaultBaseCooldown * (1 - 0.3f), subject.ActualCooldown);
        }

        [Test]
        public void FrameUpdate_WhenInBuildMode_DoesNotIncreaseTimeSinceExecuted()
        {
            gameModeManager.CurrentGameMode.Returns(GameMode.BUILD);

            var subject = ConstructSubject();

            var startTimeSinceExecuted = subject.TimeSinceExecuted;

            frameUpdater.RunUpdate(defaultDeltaTime);

            Assert.AreEqual(startTimeSinceExecuted, subject.TimeSinceExecuted);
        }

        [Test]
        public void FrameUpdate_WhenInDefendMode_IncreasesTimeSinceExecuted()
        {
            var subject = ConstructSubject();

            var startTimeSinceExecuted = subject.TimeSinceExecuted;

            frameUpdater.RunUpdate(defaultDeltaTime);

            AssertExt.Approximately(startTimeSinceExecuted + defaultDeltaTime, subject.TimeSinceExecuted);
        }

        [Test]
        public void TimeSinceExecutedClamped_AsTimePasses_MatchesTimeSinceExecutedUntilCooldownIsReachedThenMatchesCooldown()
        {
            var localBaseCooldown = 1f;

            template.BaseCooldown.Returns(localBaseCooldown);

            var subject = ConstructSubject();

            var localDeltaTime = 0.3f;

            AssertExt.Approximately(subject.TimeSinceExecuted, subject.TimeSinceExecutedClamped);

            frameUpdater.RunUpdate(localDeltaTime);

            AssertExt.Approximately(subject.TimeSinceExecuted, subject.TimeSinceExecutedClamped);

            frameUpdater.RunUpdate(localDeltaTime);

            AssertExt.Approximately(subject.TimeSinceExecuted, subject.TimeSinceExecutedClamped);

            frameUpdater.RunUpdate(localDeltaTime);

            AssertExt.Approximately(subject.TimeSinceExecuted, subject.TimeSinceExecutedClamped);

            frameUpdater.RunUpdate(localDeltaTime);

            AssertExt.Approximately(subject.ActualCooldown, subject.TimeSinceExecutedClamped);

            frameUpdater.RunUpdate(localDeltaTime);

            AssertExt.Approximately(subject.ActualCooldown, subject.TimeSinceExecutedClamped);
        }

        [Test]
        public void PercentOfCooldownPassed_AsTimePasses_IncreasesAccuratelyTo1AndThenClamps()
        {
            var localBaseCooldown = 1f;

            template.BaseCooldown.Returns(localBaseCooldown);

            var subject = ConstructSubject();

            var localDeltaTime = 0.3f;

            AssertExt.Approximately(localDeltaTime * 0 / localBaseCooldown, subject.PercentOfCooldownPassed);

            frameUpdater.RunUpdate(localDeltaTime);

            AssertExt.Approximately(localDeltaTime * 1 / localBaseCooldown, subject.PercentOfCooldownPassed);

            frameUpdater.RunUpdate(localDeltaTime);

            AssertExt.Approximately(localDeltaTime * 2 / localBaseCooldown, subject.PercentOfCooldownPassed);

            frameUpdater.RunUpdate(localDeltaTime);

            AssertExt.Approximately(localDeltaTime * 3 / localBaseCooldown, subject.PercentOfCooldownPassed);

            frameUpdater.RunUpdate(localDeltaTime);

            AssertExt.Approximately(1f, subject.PercentOfCooldownPassed);

            frameUpdater.RunUpdate(localDeltaTime);

            AssertExt.Approximately(1f, subject.PercentOfCooldownPassed);
        }

        [Test]
        public void PercentOfCooldownPassed_WhenCooldownReductionChanges_DoesNotChange()
        {
            attachedToDefender.Attributes.Get(DeAttrName.cooldownReduction).Value().Returns(0f);

            var subject = ConstructSubject();

            frameUpdater.RunUpdate(defaultDeltaTime);

            var oldPercentOfCooldownPassed = subject.PercentOfCooldownPassed;

            attachedToDefender.Attributes.Get(DeAttrName.cooldownReduction).Value().Returns(0.3f);

            attachedToDefender.Attributes.Get(DeAttrName.cooldownReduction)
                .OnAttributeChanged += Raise.EventWith(new EAOnAttributeChanged(0.3f));

            AssertExt.Approximately(oldPercentOfCooldownPassed, subject.PercentOfCooldownPassed);
        }

        [Test]
        public void TimeSinceExecuted_WhenCooldownReductionChanges_ChangesProportionally()
        {
            attachedToDefender.Attributes.Get(DeAttrName.cooldownReduction).Value().Returns(0f);

            var subject = ConstructSubject();

            frameUpdater.RunUpdate(defaultDeltaTime);

            var oldTimeSinceExecuted = subject.TimeSinceExecuted;

            attachedToDefender.Attributes.Get(DeAttrName.cooldownReduction).Value().Returns(0.3f);

            attachedToDefender.Attributes.Get(DeAttrName.cooldownReduction)
                .OnAttributeChanged += Raise.EventWith(new EAOnAttributeChanged(0.3f));

            AssertExt.Approximately(oldTimeSinceExecuted * 0.7f, subject.TimeSinceExecuted);
        }

        [Test]
        public void IsOffCooldown_AsTimePassesAndColldownIsReached_ReturnsFalseThenTrue()
        {
            var localBaseCooldown = 0.5f;

            template.BaseCooldown.Returns(localBaseCooldown);

            var subject = ConstructSubject();

            var localDeltaTime = 0.3f;

            Assert.False(subject.IsOffCooldown);

            frameUpdater.RunUpdate(localDeltaTime);

            Assert.False(subject.IsOffCooldown);

            frameUpdater.RunUpdate(localDeltaTime);

            Assert.True(subject.IsOffCooldown);
        }

        [Test]
        public void FrameUpdate_AsCooldownExpires_FiresOnAbilityOffCooldownEventOnce()
        {
            var localBaseCooldown = 0.7f;

            template.BaseCooldown.Returns(localBaseCooldown);

            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAbilityOffCooldown>();
            subject.OnAbilityOffCooldown += eventTester.Handler;

            var localDeltaTime = 0.3f;

            frameUpdater.RunUpdate(localDeltaTime);
            frameUpdater.RunUpdate(localDeltaTime);
            frameUpdater.RunUpdate(localDeltaTime);
            frameUpdater.RunUpdate(localDeltaTime);
            frameUpdater.RunUpdate(localDeltaTime);

            eventTester.AssertFired(1);
        }

        [Test]
        public void ExecuteAbility_IfFindTargetReturnsNull_ReturnsFalse()
        {
            IReadOnlyList<IDefendModeTargetable> returnVal = null;
            targetingComponent.FindTargets(attachedToDefender).Returns(returnVal);

            var subject = ConstructSubject();

            var result = subject.ExecuteAbility(attachedToDefender);

            Assert.False(result);
        }

        [Test]
        public void ExecuteAbility_IfFindTargetReturnsNull_DoesNotFireOnAbilityExecutedEvent()
        {
            IReadOnlyList<IDefendModeTargetable> returnVal = null;
            targetingComponent.FindTargets(attachedToDefender).Returns(returnVal);

            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAbilityExecuted>();
            subject.OnAbilityExecuted += eventTester.Handler;

            subject.ExecuteAbility(attachedToDefender);

            eventTester.AssertFired(false);
        }

        [Test]
        public void ExecuteAbility_WhenTargetsAreReturned_ExecutesAllEffectsWithTheCorrectArguments()
        {
            var targetList = new List<IDefendModeTargetable>();
            targetingComponent.FindTargets(attachedToDefender).Returns(targetList);

            var subject = ConstructSubject();

            subject.ExecuteAbility(attachedToDefender);

            effectComponentOne.Received(1).ExecuteEffect(attachedToDefender, targetList);
            effectComponentTwo.Received(1).ExecuteEffect(attachedToDefender, targetList);
        }

        [Test]
        public void ExecuteAbility_WhenTargetsAreReturned_ReturnsTrue()
        {
            var targetList = new List<IDefendModeTargetable>();
            targetingComponent.FindTargets(attachedToDefender).Returns(targetList);

            var subject = ConstructSubject();

            var result = subject.ExecuteAbility(attachedToDefender);

            Assert.True(result);
        }

        [Test]
        public void ExecuteAbility_WhenTargetsAreReturned_FiresOnAbilityExecutedEvent()
        {
            var targetList = new List<IDefendModeTargetable>();
            targetingComponent.FindTargets(attachedToDefender).Returns(targetList);

            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAbilityExecuted>();
            subject.OnAbilityExecuted += eventTester.Handler;

            subject.ExecuteAbility(attachedToDefender);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, x => x.DefendModeAbility == subject);
        }

        [Test]
        public void WhenOnEnterDefendModeIsFired_IfAbilityWasOnCooldown_FiresOnAbilityOffCooldownEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAbilityOffCooldown>();
            subject.OnAbilityOffCooldown += eventTester.Handler;

            gameModeManager.OnEnterDefendMode += Raise.EventWith(new EAOnEnterDefendMode());

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, x => x.DefendModeAbility == subject);
        }

        [Test]
        public void WhenOnEnterDefendModeIsFired_IfAbilityWasOnCooldown_IsNoLongerOnCooldown()
        {
            var subject = ConstructSubject();

            Assert.False(subject.IsOffCooldown);

            gameModeManager.OnEnterDefendMode += Raise.EventWith(new EAOnEnterDefendMode());

            Assert.True(subject.IsOffCooldown);
        }
    }
}