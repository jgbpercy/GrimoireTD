using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Technical;
using GrimoireTD.Attributes;
using GrimoireTD.Creeps;
using GrimoireTD.Abilities;

namespace GrimoireTD.Tests.DefendModeAbilityTests
{
    public class DefendModeAbilityTests
    {
        private float defaultBaseCooldown = 5f;

        private float defaultDeltaTime = 0.2f;

        private IReadOnlyGameModel gameModel = Substitute.For<IReadOnlyGameModel>();

        private IReadOnlyGameStateManager gameStateManager = Substitute.For<IReadOnlyGameStateManager>();

        private IReadOnlyCreepManager creepManager = Substitute.For<IReadOnlyCreepManager>();

        private IDefendingEntity attachedToDefendingEntity = Substitute.For<IDefendingEntity>();

        private IDefendModeAbilityTemplate template = Substitute.For<IDefendModeAbilityTemplate>();

        private IDefendModeTargetingComponent targetingComponent = Substitute.For<IDefendModeTargetingComponent>();

        private IDefendModeEffectComponentTemplate effectComponentTemplateOne = Substitute.For<IDefendModeEffectComponentTemplate>();
        private IDefendModeEffectComponentTemplate effectComponentTemplateTwo = Substitute.For<IDefendModeEffectComponentTemplate>();

        private IDefendModeEffectComponent effectComponentOne = Substitute.For<IDefendModeEffectComponent>();
        private IDefendModeEffectComponent effectComponentTwo = Substitute.For<IDefendModeEffectComponent>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            template.TargetingComponentTemplate.GenerateTargetingComponent().Returns(targetingComponent);

            effectComponentTemplateOne.GenerateEffectComponent().Returns(effectComponentOne);
            effectComponentTemplateTwo.GenerateEffectComponent().Returns(effectComponentTwo);

            template.EffectComponentTemplates.Returns(new List<IDefendModeEffectComponentTemplate>
            {
                effectComponentTemplateOne,
                effectComponentTemplateTwo
            });

            gameModel.GameStateManager.Returns(gameStateManager);

            gameModel.CreepManager.Returns(creepManager);

            GameModels.Models.Add(gameModel);

            GameObject modelObjectFrameUpdaterGo = new GameObject();

            modelObjectFrameUpdaterGo.AddComponent<ModelObjectFrameUpdater>();
        }

        [SetUp]
        public void EachTimeSetUp()
        {
            template.BaseCooldown.Returns(defaultBaseCooldown);

            gameStateManager.CurrentGameMode.Returns(GameMode.DEFEND);

            attachedToDefendingEntity.Attributes.Get(DEAttrName.cooldownReduction).Value().Returns(0f);

            effectComponentOne.ClearReceivedCalls();
            effectComponentTwo.ClearReceivedCalls();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(GameModels).TypeInitializer.Invoke(null, null);
        }

        private CDefendModeAbility ConstructSubject()
        {
            return new CDefendModeAbility(
                template,
                attachedToDefendingEntity
            );
        }

        [Test]
        public void ActualCooldown_WhenDefendingEntityHasNoCooldownReduction_IsBaseCooldown()
        {
            attachedToDefendingEntity.Attributes.Get(DEAttrName.cooldownReduction).Value().Returns(0f);

            var subject = ConstructSubject();

            AssertExt.Approximately(defaultBaseCooldown, subject.ActualCooldown);
        }

        [Test]
        public void ActualCooldown_WhenDefendingEntityHasCooldownReduction_TakesReductionIntoAcount()
        {
            attachedToDefendingEntity.Attributes.Get(DEAttrName.cooldownReduction).Value().Returns(0.2f);

            var subject = ConstructSubject();

            AssertExt.Approximately(defaultBaseCooldown * (1 - 0.2f), subject.ActualCooldown);
        }

        [Test]
        public void ActualCooldown_WhenDefendingEntityCooldownReductionChanges_TakesReductionIntoAccount()
        {
            attachedToDefendingEntity.Attributes.Get(DEAttrName.cooldownReduction).Value().Returns(0f);

            var subject = ConstructSubject();

            attachedToDefendingEntity.Attributes.Get(DEAttrName.cooldownReduction).Value().Returns(0.3f);

            attachedToDefendingEntity.Attributes.Get(DEAttrName.cooldownReduction)
                .OnAttributeChanged += Raise.EventWith(new EAOnAttributeChanged(0.3f));

            AssertExt.Approximately(defaultBaseCooldown * (1 - 0.3f), subject.ActualCooldown);
        }

        [Test]
        public void ModelObjectFrameUpdate_WhenInBuildMode_DoesNotIncreaseTimeSinceExecuted()
        {
            gameStateManager.CurrentGameMode.Returns(GameMode.BUILD);

            var subject = ConstructSubject();

            var startTimeSinceExecuted = subject.TimeSinceExecuted;

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            Assert.AreEqual(startTimeSinceExecuted, subject.TimeSinceExecuted);
        }

        [Test]
        public void ModelObjectFrameUpdate_WhenInDefendMode_IncreasesTimeSinceExecuted()
        {
            var subject = ConstructSubject();

            var startTimeSinceExecuted = subject.TimeSinceExecuted;

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

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

            subject.ModelObjectFrameUpdate(localDeltaTime);

            AssertExt.Approximately(subject.TimeSinceExecuted, subject.TimeSinceExecutedClamped);

            subject.ModelObjectFrameUpdate(localDeltaTime);

            AssertExt.Approximately(subject.TimeSinceExecuted, subject.TimeSinceExecutedClamped);

            subject.ModelObjectFrameUpdate(localDeltaTime);

            AssertExt.Approximately(subject.TimeSinceExecuted, subject.TimeSinceExecutedClamped);

            subject.ModelObjectFrameUpdate(localDeltaTime);

            AssertExt.Approximately(subject.ActualCooldown, subject.TimeSinceExecutedClamped);

            subject.ModelObjectFrameUpdate(localDeltaTime);

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

            subject.ModelObjectFrameUpdate(localDeltaTime);

            AssertExt.Approximately(localDeltaTime * 1 / localBaseCooldown, subject.PercentOfCooldownPassed);

            subject.ModelObjectFrameUpdate(localDeltaTime);

            AssertExt.Approximately(localDeltaTime * 2 / localBaseCooldown, subject.PercentOfCooldownPassed);

            subject.ModelObjectFrameUpdate(localDeltaTime);

            AssertExt.Approximately(localDeltaTime * 3 / localBaseCooldown, subject.PercentOfCooldownPassed);

            subject.ModelObjectFrameUpdate(localDeltaTime);

            AssertExt.Approximately(1f, subject.PercentOfCooldownPassed);

            subject.ModelObjectFrameUpdate(localDeltaTime);

            AssertExt.Approximately(1f, subject.PercentOfCooldownPassed);
        }

        [Test]
        public void PercentOfCooldownPassed_WhenCooldownReductionChanges_DoesNotChange()
        {
            attachedToDefendingEntity.Attributes.Get(DEAttrName.cooldownReduction).Value().Returns(0f);

            var subject = ConstructSubject();

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            var oldPercentOfCooldownPassed = subject.PercentOfCooldownPassed;

            attachedToDefendingEntity.Attributes.Get(DEAttrName.cooldownReduction).Value().Returns(0.3f);

            attachedToDefendingEntity.Attributes.Get(DEAttrName.cooldownReduction)
                .OnAttributeChanged += Raise.EventWith(new EAOnAttributeChanged(0.3f));

            AssertExt.Approximately(oldPercentOfCooldownPassed, subject.PercentOfCooldownPassed);
        }

        [Test]
        public void TimeSinceExecuted_WhenCooldownReductionChanges_ChangesProportionally()
        {
            attachedToDefendingEntity.Attributes.Get(DEAttrName.cooldownReduction).Value().Returns(0f);

            var subject = ConstructSubject();

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            var oldTimeSinceExecuted = subject.TimeSinceExecuted;

            attachedToDefendingEntity.Attributes.Get(DEAttrName.cooldownReduction).Value().Returns(0.3f);

            attachedToDefendingEntity.Attributes.Get(DEAttrName.cooldownReduction)
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

            subject.ModelObjectFrameUpdate(localDeltaTime);

            Assert.False(subject.IsOffCooldown);

            subject.ModelObjectFrameUpdate(localDeltaTime);

            Assert.True(subject.IsOffCooldown);
        }

        [Test]
        public void ModelObjectFrameUpdate_AsCooldownExpires_FiresOnAbilityOffCooldownEventOnce()
        {
            var localBaseCooldown = 0.7f;

            template.BaseCooldown.Returns(localBaseCooldown);

            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAbilityOffCooldown>();
            subject.OnAbilityOffCooldown += eventTester.Handler;

            var localDeltaTime = 0.3f;

            subject.ModelObjectFrameUpdate(localDeltaTime);
            subject.ModelObjectFrameUpdate(localDeltaTime);
            subject.ModelObjectFrameUpdate(localDeltaTime);
            subject.ModelObjectFrameUpdate(localDeltaTime);
            subject.ModelObjectFrameUpdate(localDeltaTime);

            eventTester.AssertFired(1);
        }

        [Test]
        public void ExecuteAbility_IfFindTargetReturnsNull_ReturnsFalse()
        {
            IReadOnlyList<IDefendModeTargetable> returnVal = null;
            targetingComponent.FindTargets(attachedToDefendingEntity, Arg.Any<IReadOnlyList<ICreep>>()).Returns(returnVal);

            var subject = ConstructSubject();

            var result = subject.ExecuteAbility(attachedToDefendingEntity);

            Assert.False(result);
        }

        [Test]
        public void ExecuteAbility_IfFindTargetReturnsNull_DoesNotFireOnAbilityExecutedEvent()
        {
            IReadOnlyList<IDefendModeTargetable> returnVal = null;
            targetingComponent.FindTargets(attachedToDefendingEntity, Arg.Any<IReadOnlyList<ICreep>>()).Returns(returnVal);

            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAbilityExecuted>();
            subject.OnAbilityExecuted += eventTester.Handler;

            subject.ExecuteAbility(attachedToDefendingEntity);

            eventTester.AssertFired(false);
        }

        [Test]
        public void ExecuteAbility_WhenTargetsAreReturned_ExecutesAllEffectsWithTheCorrectArguments()
        {
            var targetList = new List<IDefendModeTargetable>();
            targetingComponent.FindTargets(attachedToDefendingEntity, Arg.Any<IReadOnlyList<ICreep>>()).Returns(targetList);

            var subject = ConstructSubject();

            subject.ExecuteAbility(attachedToDefendingEntity);

            effectComponentOne.Received(1).ExecuteEffect(attachedToDefendingEntity, targetList);
            effectComponentTwo.Received(1).ExecuteEffect(attachedToDefendingEntity, targetList);
        }

        [Test]
        public void ExecuteAbility_WhenTargetsAreReturned_ReturnsTrue()
        {
            var targetList = new List<IDefendModeTargetable>();
            targetingComponent.FindTargets(attachedToDefendingEntity, Arg.Any<IReadOnlyList<ICreep>>()).Returns(targetList);

            var subject = ConstructSubject();

            var result = subject.ExecuteAbility(attachedToDefendingEntity);

            Assert.True(result);
        }

        [Test]
        public void ExecuteAbility_WhenTargetsAreReturned_FiresOnAbilityExecutedEvent()
        {
            var targetList = new List<IDefendModeTargetable>();
            targetingComponent.FindTargets(attachedToDefendingEntity, Arg.Any<IReadOnlyList<ICreep>>()).Returns(targetList);

            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAbilityExecuted>();
            subject.OnAbilityExecuted += eventTester.Handler;

            subject.ExecuteAbility(attachedToDefendingEntity);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, x => x.DefendModeAbility == subject);
        }

        [Test]
        public void WhenOnEnterDefendModeIsFired_IfAbilityWasOnCooldown_FiresOnAbilityOffCooldownEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAbilityOffCooldown>();
            subject.OnAbilityOffCooldown += eventTester.Handler;

            gameStateManager.OnEnterDefendMode += Raise.EventWith(new EAOnEnterDefendMode());

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, x => x.DefendModeAbility == subject);
        }

        [Test]
        public void WhenOnEnterDefendModeIsFired_IfAbilityWasOnCooldown_IsNoLongerOnCooldown()
        {
            var subject = ConstructSubject();

            Assert.False(subject.IsOffCooldown);

            gameStateManager.OnEnterDefendMode += Raise.EventWith(new EAOnEnterDefendMode());

            Assert.True(subject.IsOffCooldown);
        }
    }
}