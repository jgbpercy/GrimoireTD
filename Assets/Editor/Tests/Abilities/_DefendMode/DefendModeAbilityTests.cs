using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Tests.DefendModeAbilityTests
{
    public class DefendModeAbilityTests
    {
        private IDefendingEntity attachedToDefendingEntity;

        private IDefendModeAbilityTemplate template;

        private CDefendModeAbility subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            attachedToDefendingEntity = Substitute.For<IDefendingEntity>();

            template = Substitute.For<IDefendModeAbilityTemplate>();

            subject = new CDefendModeAbility(template, attachedToDefendingEntity);
        }

        [Test]
        public void ActualCooldown_WhenDefendingEntityHasNoCooldownReduction_IsBaseCooldown()
        {

        }

        [Test]
        public void ActualCooldown_WhenDefendingEntityHasCooldownReduction_TakesReductionIntoAcount()
        {

        }

        [Test]
        public void ActualCooldown_WhenDefendingEntityCooldownReductionChanges_TakesReductionIntoAccount()
        {

        }

        [Test]
        public void ModelObjectFrameUpdate_WhenInBuildMode_DoesNotIncreaseTimeSinceExecuted()
        {

        }

        [Test]
        public void ModelObjectFrameUpdate_WhenInDefendMode_IncreasesTimeSinceExecuted()
        {

        }

        [Test]
        public void TimeSinceExecutedClamped_BeforeCooldownHasPassed_MatchesTimeSinceExecuted()
        {

        }

        [Test]
        public void TimeSinceExecutedClamped_AfterCooldownHasPassed_MatchesActualCooldown()
        {

        }

        [Test]
        public void PercentOfCooldownPassed_AsTimePasses_IncreasesAccuratelyTo1AndThenClamps()
        {

        }

        [Test]
        public void PercentOfCooldownPassed_WhenCooldownReductionChanges_DoesNotChange()
        {

        }

        [Test]
        public void TimeSinceExecuted_WhenCooldownReductionChanges_ChangesProportionally()
        {

        }

        [Test]
        public void IsOffCooldown_BeforeCooldownHasPassed_ReturnsFalse()
        {

        }

        [Test]
        public void IsOffCoolDown_AfterCooldownHasPassed_ReturnsTrue()
        {

        }

        [Test]
        public void ModelObjectFrameUpdate_AsCooldownExpires_FiresOnAbilityOffCooldownEventOnce()
        {

        }

        [Test]
        public void ExecuteAbility_IfFindTargetReturnsNull_ReturnsFalse()
        {

        }

        [Test]
        public void ExecuteAbility_IfFindTargetReturnsNull_DoesNotFireOnAbilityExecutedEvent()
        {

        }

        [Test]
        public void ExecuteAbility_WhenTargetsAreReturned_ExecutesAllEffectsWithThePassedDefendingEntity()
        {

        }

        [Test]
        public void ExecuteAbility_WhenTargetsAreReturned_ExecutesAllEffectsWithReturnedTargets()
        {

        }

        [Test]
        public void ExecuteAbility_WhenTargetsAreReturned_ReturnsTrue()
        {

        }

        [Test]
        public void ExecuteAbility_WhenTargetsAreReturned_FiresOnAbilityExecutedEvent()
        {

        }

        [Test]
        public void WhenOnEnterDefendModeIsFired_IfThereWereAbilitiesOnCooldow_FiresOnAbilityOffCooldownEventOnceForEachAbility()
        {

        }
    }
}