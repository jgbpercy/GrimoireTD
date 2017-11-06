using NUnit.Framework;
using NSubstitute;
using GrimoireTD.DefendingEntities;
using GrimoireTD.DefendingEntities.Units;
using GrimoireTD.Economy;
using System.Collections.Generic;
using GrimoireTD.DefendingEntities.Structures;

namespace GrimoireTD.Tests.UnitTests
{
    public class UnitTests : DefendingEntityTests.DefendingEntityTests
    {
        //Primitives and Basic Objects
        private int matchingConditionalHexOccupationBonus1Resource1Amount = 5;
        private int matchingConditionalHexOccupationBonus1Resource2Amount = 6;

        private int matchingConditionalHexOccupationBonus2Resource1Amount = 7;
        private int matchingConditionalHexOccupationBonus2Resource2Amount = 8;

        private int otherConditionalHexOccupationBonusResource1Amount = 9;
        private int otherConditionalHexOccupationBonusResource2Amount = 10;

        private int matchingConditionalStructureOccupationBonus1Resource1Amount = 11;
        private int matchingConditionalStructureOccupationBonus1Resource2Amount = 12;

        private int matchingConditionalStructureOccupationBonus2Resource1Amount = 13;
        private int matchingConditionalStructureOccupationBonus2Resource2Amount = 14;

        private int wrongStructureConditionalStructureOccupationBonusResource1Amount = 15;
        private int wrongStructureConditionalStructureOccupationBonusResource2Amount = 16;

        private int wrongUpgradeLevelConditionalStructureOccupationBonusResource1Amount = 17;
        private int wrongUpgradeLevelConditionalStructureOccupationBonusResource2Amount = 18;

        private int experienceToLevelUp = 100;

        //Model and Frame Updater


        //Instance Dependency Provider Deps


        //Template Deps
        private IUnitTemplate template = Substitute.For<IUnitTemplate>();

        private IUnitImprovement baseCharacteristics = Substitute.For<IUnitImprovement>();

        private IEconomyTransaction matchingConditionalHexOccupationBonus1Transaction;
        private IEconomyTransaction matchingConditionalHexOccupationBonus2Transaction;
        private IEconomyTransaction otherConditionalHexOccupationBonusTransaction;

        private IHexOccupationBonus matchingConditionalHexOccupationBonus1 = Substitute.For<IHexOccupationBonus>();
        private IHexOccupationBonus matchingConditionalHexOccupationBonus2 = Substitute.For<IHexOccupationBonus>();
        private IHexOccupationBonus otherConditionalHexOccupationBonus = Substitute.For<IHexOccupationBonus>();

        private IEconomyTransaction matchingConditionalStructureOccupationBonus1Transaction;
        private IEconomyTransaction matchingConditionalStructureOccupationBonus2Transaction;
        private IEconomyTransaction wrongStructureConditionalStructureOccupationBonusTransaction;
        private IEconomyTransaction wrongUpgradeLevelConditionalStructureOccupationBonusTransaction;

        private IStructureOccupationBonus matchingConditionalStructureOccupationBonus1 = Substitute.For<IStructureOccupationBonus>();
        private IStructureOccupationBonus matchingConditionalStructureOccupationBonus2 = Substitute.For<IStructureOccupationBonus>();
        private IStructureOccupationBonus wrongStructureConditionalStructureOccupationBonus = Substitute.For<IStructureOccupationBonus>();
        private IStructureOccupationBonus wrongUpgradeLevelConditionalStructureOccupationBonus = Substitute.For<IStructureOccupationBonus>();

        private IStructure structureOnHex = Substitute.For<IStructure>();
        private IStructureTemplate structureOnHexTemplate = Substitute.For<IStructureTemplate>();
        private IStructureUpgrade structureOnHexUpgradeLevel = Substitute.For<IStructureUpgrade>();
        private IStructureUpgrade structureOnHexOtherUpgrade = Substitute.For<IStructureUpgrade>();

        private IStructure otherStructure = Substitute.For<IStructure>();
        private IStructureTemplate otherStructureTemplate = Substitute.For<IStructureTemplate>();
        private IStructureUpgrade otherStructureUpgradeLevel = Substitute.For<IStructureUpgrade>();

        //Other Deps Passed To Ctor or SetUp


        //Other Objects Passed To Methods


        [OneTimeSetUp]
        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();

            //Model and Frame Updater


            //Instance Dependency Provider Deps


            //Template Deps
            template.BaseCharacteristics.Returns(baseCharacteristics);

            SetUpBaseCharacteristics(baseCharacteristics);

            structureOnHex.StructureTemplate.Returns(structureOnHexTemplate);
            structureOnHex.CurrentUpgradeLevel().Returns(structureOnHexUpgradeLevel);

            otherStructure.StructureTemplate.Returns(otherStructureTemplate);
            otherStructure.CurrentUpgradeLevel().Returns(otherStructureUpgradeLevel);

            template.ExperienceToLevelUp.Returns(experienceToLevelUp);

            //Other Deps Passed To Ctor or SetUp


            //Other Objects Passed To Methods


        }

        [SetUp]
        public override void EachTestSetUp()
        {
            base.EachTestSetUp();
        }

        protected override CDefendingEntity ConstructSubject()
        {
            return new CUnit(template, startPosition);
        }

        private CUnit ConstructUnitSubject()
        {
            return ConstructSubject() as CUnit;
        }

        protected override void SetUpBaseCharacteristics(IDefendingEntityImprovement baseCharacteristics)
        {
            base.SetUpBaseCharacteristics(baseCharacteristics);

            matchingConditionalHexOccupationBonus1Transaction = new CEconomyTransaction(new List<IResourceTransaction>
            {
                new CResourceTransaction(resource1, matchingConditionalHexOccupationBonus1Resource1Amount),
                new CResourceTransaction(resource2, matchingConditionalHexOccupationBonus1Resource2Amount)
            });

            matchingConditionalHexOccupationBonus2Transaction = new CEconomyTransaction(new List<IResourceTransaction>
            {
                new CResourceTransaction(resource1, matchingConditionalHexOccupationBonus2Resource1Amount),
                new CResourceTransaction(resource2, matchingConditionalHexOccupationBonus2Resource2Amount)
            });

            otherConditionalHexOccupationBonusTransaction = new CEconomyTransaction(new List<IResourceTransaction>
            {
                new CResourceTransaction(resource1, otherConditionalHexOccupationBonusResource1Amount),
                new CResourceTransaction(resource2, otherConditionalHexOccupationBonusResource2Amount)
            });

            matchingConditionalHexOccupationBonus1.ResourceGain.Returns(matchingConditionalHexOccupationBonus1Transaction);
            matchingConditionalHexOccupationBonus2.ResourceGain.Returns(matchingConditionalHexOccupationBonus2Transaction);
            otherConditionalHexOccupationBonus.ResourceGain.Returns(otherConditionalHexOccupationBonusTransaction);

            matchingConditionalHexOccupationBonus1.HexType.Returns(occupiedHexType);
            matchingConditionalHexOccupationBonus2.HexType.Returns(occupiedHexType);
            otherConditionalHexOccupationBonus.HexType.Returns(otherHexType);

            matchingConditionalStructureOccupationBonus1Transaction = new CEconomyTransaction(new List<IResourceTransaction>
            {
                new CResourceTransaction(resource1, matchingConditionalStructureOccupationBonus1Resource1Amount),
                new CResourceTransaction(resource2, matchingConditionalStructureOccupationBonus1Resource2Amount)
            });

            matchingConditionalStructureOccupationBonus2Transaction = new CEconomyTransaction(new List<IResourceTransaction>
            {
                new CResourceTransaction(resource1, matchingConditionalStructureOccupationBonus2Resource1Amount),
                new CResourceTransaction(resource2, matchingConditionalStructureOccupationBonus2Resource2Amount)
            });

            wrongStructureConditionalStructureOccupationBonusTransaction = new CEconomyTransaction(new List<IResourceTransaction>
            {
                new CResourceTransaction(resource1, wrongStructureConditionalStructureOccupationBonusResource1Amount),
                new CResourceTransaction(resource2, wrongStructureConditionalStructureOccupationBonusResource2Amount)
            });

            wrongUpgradeLevelConditionalStructureOccupationBonusTransaction = new CEconomyTransaction(new List<IResourceTransaction>
            {
                new CResourceTransaction(resource1, wrongUpgradeLevelConditionalStructureOccupationBonusResource1Amount),
                new CResourceTransaction(resource2, wrongUpgradeLevelConditionalStructureOccupationBonusResource2Amount)
            });

            matchingConditionalStructureOccupationBonus1.ResourceGain.Returns(matchingConditionalStructureOccupationBonus2Transaction);
            matchingConditionalStructureOccupationBonus2.ResourceGain.Returns(matchingConditionalStructureOccupationBonus2Transaction);
            wrongStructureConditionalStructureOccupationBonus.ResourceGain.Returns(wrongStructureConditionalStructureOccupationBonusTransaction);
            wrongUpgradeLevelConditionalStructureOccupationBonus.ResourceGain.Returns(wrongUpgradeLevelConditionalStructureOccupationBonusTransaction);

            matchingConditionalStructureOccupationBonus1.StructureTemplate.Returns(structureOnHexTemplate);
            matchingConditionalStructureOccupationBonus2.StructureTemplate.Returns(structureOnHexTemplate);
            wrongStructureConditionalStructureOccupationBonus.StructureTemplate.Returns(otherStructureTemplate);
            wrongUpgradeLevelConditionalStructureOccupationBonus.StructureTemplate.Returns(structureOnHexTemplate);

            matchingConditionalStructureOccupationBonus1.StructureUpgradeLevel.Returns(structureOnHexUpgradeLevel);
            matchingConditionalStructureOccupationBonus2.StructureUpgradeLevel.Returns(structureOnHexUpgradeLevel);
            wrongStructureConditionalStructureOccupationBonus.StructureUpgradeLevel.Returns(otherStructureUpgradeLevel);
            wrongUpgradeLevelConditionalStructureOccupationBonus.StructureUpgradeLevel.Returns(structureOnHexOtherUpgrade);
        }

        [Test]
        public void Ctor_Always_CreatesUnitWithZeroTimeIdle()
        {

        }

        [Test]
        public void Ctor_Always_CreatesUnitWithZeroTimeActive()
        {

        }

        [Test]
        public void Ctor_Always_CreateUnitWithZeroExperience()
        {

        }

        [Test]
        public void Ctor_Always_CreatesUnitWithZeroFatigue()
        {

        }

        [Test]
        public void Ctor_Always_CreatesUnitWithZeroPendingLevelUps()
        {

        }

        [Test]
        public void Ctor_Always_CreatesUnitAtLevelZero()
        {

        }

        [Test]
        public void Ctor_Always_AddsBaseCharacteristicsConditionalHexOccupationBonus()
        {

        }

        [Test]
        public void Ctor_Always_AddsBaseCharacteristicsConditionalStructureOccupationBonus()
        {

        }

        [Test]
        public void LevelledTalents_AfterConstruction_ContainsAllAvailableTalentsAtLevelZero()
        {

        }

        [Test]
        public void CurrentName_Always_ReturnsNameFromTemplate()
        {

        }

        [Test]
        public void UIText_Always_ReturnsDescriptionFromTemplate()
        {

        }

        [Test]
        public void Ctor_IfStartingHexContainsAnAuraThatAffectsUnits_AppliesAura()
        {

        }

        [Test]
        public void OnAuraAddedToHex_IfAuraAffectsUnits_AppliesAura()
        {

        }

        [Test]
        public void FrameUpdate_InBuildMode_DoesNotChangeTimeActiveOrTimeIdle()
        {

        }

        [Test]
        public void FrameUpdate_WhenNoAbilityExecuted_AddsDeltaTimeToTimeIdle()
        {

        }

        [Test]
        public void FrameUpdate_AfterAbilityExecutedAndBeforeOffCooldown_AddsDeltaTimeToTimeActive()
        {

        }

        [Test]
        public void FrameUpdate_AfterAbilityExecutedAndAllAbilitiesOffCooldownAgain_AddsDeltaTimeToTimeIdle()
        {

        }

        [Test]
        public void OnEnterBuildMode_Always_ResetsTimeActiveAndTimeIdle()
        {

        }

        [Test]
        public void OnEnterBuildMode_Always_TriggersOnConditionalOccupationBonusesEventWithConditionalHexBonusProportionalToTimeActive()
        {

        }

        [Test]
        public void OnEnterBuildMode_Always_TriggersOnConditionalOccupationBonusEventWithConditionalStructureBonusProportionalToTimeActive()
        {

        }

        [Test]
        public void OnEnterBuildMode_AfterOneRound_AddsExperienceProportionalToTimeActive()
        {

        }

        [Test]
        public void OnEnterBuildMode_AfterOneRound_AddsFatigueProportionalToTimeActive()
        {

        }

        [Test]
        public void OnEnterBuildMode_AfterSecondRound_AddsExperienceTakingIntoAccountTimeActiveAndFatigue()
        {

        }

        [Test]
        public void OnEnterBuildMode_AfterSecondRoundWhenLessThan50PercentActive_ReducesFatigue()
        {

        }

        [Test]
        public void OnEnterBuildMode_Always_HasMinimumFatigueZero()
        {

        }

        [Test]
        public void OnEnterBuildMode_Always_FiresOnExperienceFatigueLevelChangedEventWithCorrectValues()
        {

        }

        [Test]
        public void OnEnterBuildMode_IfEnoughTotalExperienceGainedToLevelUp_IncreasesLevelUpsPending()
        {

        }

        [Test]
        public void TryLevelUp_IfNotEnoughExperienceGainedYet_ReturnsFalse()
        {

        }

        [Test]
        public void TryLevelUp_IfAlreadyLevelledTalentToMax_ReturnsFalse()
        {

        }

        [Test]
        public void TryLevelUp_IfLevelUpPendingAndTalentNotMaxed_IncrementsLevelledTalentsForThatTalent()
        {

        }

        [Test]
        public void TryLevelUp_IfLevelUpPendingAndTalentNotMaxed_IncreasesLevel()
        {

        }

        [Test]
        public void TryLevelUp_IfLevelUpPendingAndTalentNotMaxed_DecreasesLevelUpsPending()
        {

        }

        [Test]
        public void TryLevelUp_IfLevelUpPendingAndTalentNotMaxed_ReturnsTrue()
        {

        }

        [Test]
        public void TryLevelUp_IfLevelUpPendingAndTalentNotMaxed_FiresOnExperienceFatigueLevelChangedEvent()
        {

        }

        [Test]
        public void TryLevelUp_IfTalentNotLevelledUpYet_AppliesTheImprovementForTheFirstLevel()
        {

        }

        [Test]
        public void TryLevelUp_IfTalentAlreadyLevelledUp_RemovedPreviousLevelImprovementAndAppliesNewLevelImprovement()
        {

        }

        [Test]
        public void Move_Always_ChangesPositionToTargetCoord()
        {

        }

        [Test]
        public void Move_Always_FiresOnMovedEvent()
        {

        }

        [Test]
        public void Move_IfUnitHasEmittedAuras_ClearsAuras()
        {

        }

        [Test]
        public void Move_IfUnitHasEmittedAura_InitialisesAurasAtNewPosition()
        {

        }

        [Test]
        public void Move_IfUnitIsAffectedByAurasThatAreNotAtNewHex_RemovesThoseAuras()
        {

        }

        [Test]
        public void Move_IfNewPositionHasAuraThatAffectBoth_AppliesAura()
        {

        }

        [Test]
        public void Move_IfNewPositionHasAuraThatAffectsUnits_AppliesAura()
        {

        }

        [Test]
        public void OnRelevantDefenderAuraAddedToOldPosition_AfterMove_DoesNotApplyAura()
        {

        }

        [Test]
        public void OnRelevantDefenderAuraAddedToNewPosition_AfterMove_AppliesAura()
        {

        }

        [Test]
        public void OnRelevantDefenderAuraRemovedFromNewPosition_AfterMove_RemovesAura()
        {

        }

        [Test]
        public void RegenerateCachedDisallowedMovementDestinations_Always_GetsDisallowedMovementDestinationsFromMapForCurrentPosition()
        {

        }
    }
}