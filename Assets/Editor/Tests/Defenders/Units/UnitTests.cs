using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Defenders;
using GrimoireTD.Defenders.Units;
using GrimoireTD.Economy;
using System.Collections.Generic;
using GrimoireTD.Defenders.Structures;
using System.Linq;
using GrimoireTD.Defenders.DefenderEffects;
using GrimoireTD.Map;
using GrimoireTD.Attributes;
using GrimoireTD.Dependencies;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.Abilities;
using UnityEngine;
using GrimoireTD.Technical;

namespace GrimoireTD.Tests.UnitTests
{
    public class UnitTests : DefenderTests.DefenderTests
    {
        //Primitives and Basic Objects
        private float defaultDeltaTime = 0.02f;
            
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

        private string unitName = "Unit Name";
        private string unitDescription = "Unit Description";

        private float fatigueInflectionPoint = 8;
        private float fatigueShallownessMultiplier = 80;

        private int talent1Level1DefenderAuraRange = 1;

        private Coord movementTarget = new Coord(3, 3);

        //Model and Frame Updater
        private FrameUpdaterStub frameUpdater;

        private IHexData movedToHexData = Substitute.For<IHexData>(); 

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

        private IUnitTalent talent1 = Substitute.For<IUnitTalent>();
        private IUnitTalent talent2 = Substitute.For<IUnitTalent>();

        private IUnitImprovement talent1Level1 = Substitute.For<IUnitImprovement>();
        private IUnitImprovement talent1Level2 = Substitute.For<IUnitImprovement>();

        private IUnitImprovement talent2Level1 = Substitute.For<IUnitImprovement>();
        private IUnitImprovement talent2Level2 = Substitute.For<IUnitImprovement>();

        private IAbilityTemplate talent1Level1AbilityTemplate = Substitute.For<IAbilityTemplate>();
        private IAbilityTemplate talent1Level2AbilityTemplate = Substitute.For<IAbilityTemplate>();

        private IAbility talent1Level1Ability = Substitute.For<IAbility>();
        private IAbility talent1Level2Ability = Substitute.For<IAbility>();

        private INamedAttributeModifier<DeAttrName> talent1Level1AttributeModifier = Substitute.For<INamedAttributeModifier<DeAttrName>>();
        private IHexOccupationBonus talent1Level1FlatHexOccupationBonus = Substitute.For<IHexOccupationBonus>();
        private IDefenderAuraTemplate talent1Level1DefenderAuraTemplate = Substitute.For<IDefenderAuraTemplate>();
        private IDefenderAura talent1Level1DefenderAura = Substitute.For<IDefenderAura>();
        private IHexOccupationBonus talent1Level1ConditionalHexOccupationBonus = Substitute.For<IHexOccupationBonus>();
        private IStructureOccupationBonus talent1Level1ConditionalStructureOccupationBonus = Substitute.For<IStructureOccupationBonus>();
        
        [OneTimeSetUp]
        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();

            //Model and Frame Updater
            DepsProv.TheModelObjectFrameUpdater = () => frameUpdater;

            gameModel.IsSetUp.Returns(true);
            gameModel.UnitFatigueFactorInfelctionPoint.Returns(fatigueInflectionPoint);
            gameModel.UnitFatigueFactorShallownessMultiplier.Returns(fatigueShallownessMultiplier);

            //Template Deps
            template.BaseCharacteristics.Returns(baseCharacteristics);

            SetUpBaseCharacteristics(baseCharacteristics);

            structureOnHex.StructureTemplate.Returns(structureOnHexTemplate);
            structureOnHex.CurrentUpgradeLevel().Returns(structureOnHexUpgradeLevel);

            otherStructure.StructureTemplate.Returns(otherStructureTemplate);
            otherStructure.CurrentUpgradeLevel().Returns(otherStructureUpgradeLevel);

            template.ExperienceToLevelUp.Returns(experienceToLevelUp);

            template.UnitTalents.Returns(new List<IUnitTalent>
            {
                talent1,
                talent2
            });

            talent1.UnitImprovements.Returns(new List<IUnitImprovement>
            {
                talent1Level1,
                talent1Level2
            });

            talent2.UnitImprovements.Returns(new List<IUnitImprovement>
            {
                talent2Level1,
                talent2Level2
            });

            talent1Level1.Abilities.Returns(new List<IAbilityTemplate> { talent1Level1AbilityTemplate });
            talent1Level2.Abilities.Returns(new List<IAbilityTemplate> { talent1Level2AbilityTemplate });
            talent1Level1AbilityTemplate.GenerateAbility(Arg.Any<IDefender>()).Returns(talent1Level1Ability);
            talent1Level2AbilityTemplate.GenerateAbility(Arg.Any<IDefender>()).Returns(talent1Level2Ability);

            talent1Level1.AttributeModifiers.Returns(new List<INamedAttributeModifier<DeAttrName>> { talent1Level1AttributeModifier });

            talent1Level1.Auras.Returns(new List<IDefenderAuraTemplate> { talent1Level1DefenderAuraTemplate });
            talent1Level1DefenderAuraTemplate.GenerateDefenderAura(Arg.Any<IDefender>()).Returns(talent1Level1DefenderAura);
            talent1Level1DefenderAura.Range.Returns(talent1Level1DefenderAuraRange);

            mapData.GetCoordsInRange(talent1Level1DefenderAuraRange, startPosition).Returns(new List<Coord> { startPosition });

            talent1Level1.FlatHexOccupationBonuses.Returns(new List<IHexOccupationBonus> { talent1Level1FlatHexOccupationBonus });

            talent1Level1.ConditionalHexOccupationBonuses.Returns(new List<IHexOccupationBonus> { talent1Level1ConditionalHexOccupationBonus });

            talent1Level1.ConditionalStructureOccupationBonuses.Returns(new List<IStructureOccupationBonus> { talent1Level1ConditionalStructureOccupationBonus });

            template.NameInGame.Returns(unitName);
            template.Description.Returns(unitDescription);
        }

        [SetUp]
        public override void EachTestSetUp()
        {
            base.EachTestSetUp();

            frameUpdater = new FrameUpdaterStub();

            startHexData.StructureHere.Returns(structureOnHex);

            movedToHexData.ClearReceivedCalls();

            mapData.GetHexAt(movementTarget).Returns(movedToHexData);
            mapData.GetCoordsInRange(defenderAuraRange, movementTarget).Returns(new List<Coord> { movementTarget });
        }

        protected override CDefender ConstructSubject()
        {
            return new CUnit(template, startPosition);
        }

        private CUnit ConstructUnitSubject()
        {
            return ConstructSubject() as CUnit;
        }

        protected override void SetUpBaseCharacteristics(IDefenderImprovement baseCharacteristics)
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

            ((IUnitImprovement)baseCharacteristics).ConditionalHexOccupationBonuses.Returns(new List<IHexOccupationBonus>
            {
                matchingConditionalHexOccupationBonus1,
                matchingConditionalHexOccupationBonus2,
                otherConditionalHexOccupationBonus
            });

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

            matchingConditionalStructureOccupationBonus1.ResourceGain.Returns(matchingConditionalStructureOccupationBonus1Transaction);
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

            ((IUnitImprovement)baseCharacteristics).ConditionalStructureOccupationBonuses.Returns(new List<IStructureOccupationBonus>
            {
                matchingConditionalStructureOccupationBonus1,
                matchingConditionalStructureOccupationBonus2,
                wrongStructureConditionalStructureOccupationBonus,
                wrongUpgradeLevelConditionalStructureOccupationBonus
            });
        }

        private void SimulateRound(
            float timeIdle, 
            float timeActive, 
            bool revertToBuildMode,
            bool endWithAllAbilitiesOffCooldown = true
        )
        {
            gameStateManager.CurrentGameMode.Returns(GameMode.DEFEND);
            gameStateManager.OnEnterDefendMode += Raise.EventWith<EAOnEnterDefendMode>();

            var defendModeAbility = Substitute.For<IDefendModeAbility>();
            abilities.OnDefendModeAbilityAdded += Raise.EventWith(new EAOnDefendModeAbilityAdded(defendModeAbility));

            frameUpdater.RunUpdate(timeIdle);

            defendModeAbility.OnAbilityExecuted += Raise.EventWith(new EAOnAbilityExecuted(defendModeAbility));

            frameUpdater.RunUpdate(timeActive);

            if (endWithAllAbilitiesOffCooldown)
            {
                abilities.DefendModeAbilityManager.OnAllDefendModeAbilitiesOffCooldown += Raise.EventWith(new EAOnAllDefendModeAbilitiesOffCooldown());
            }

            if (revertToBuildMode)
            {
                gameStateManager.CurrentGameMode.Returns(GameMode.BUILD);
                gameStateManager.OnEnterBuildMode += Raise.EventWith<EAOnEnterBuildMode>();
            }
        }

        [Test]
        public void Ctor_Always_CreatesUnitWithZeroTimeIdle()
        {
            var subject = ConstructUnitSubject();

            AssertExt.Approximately(0f, subject.TimeIdle);
        }

        [Test]
        public void Ctor_Always_CreatesUnitWithZeroTimeActive()
        {
            var subject = ConstructUnitSubject();

            AssertExt.Approximately(0f, subject.TimeActive);
        }

        [Test]
        public void Ctor_Always_CreateUnitWithZeroExperience()
        {
            var subject = ConstructUnitSubject();

            Assert.AreEqual(0, subject.Experience);
        }

        [Test]
        public void Ctor_Always_CreatesUnitWithZeroFatigue()
        {
            var subject = ConstructUnitSubject();

            Assert.AreEqual(0, subject.Fatigue);
        }

        [Test]
        public void Ctor_Always_CreatesUnitWithZeroPendingLevelUps()
        {
            var subject = ConstructUnitSubject();

            Assert.AreEqual(0, subject.LevelUpsPending);
        }

        [Test]
        public void Ctor_Always_CreatesUnitAtLevelZero()
        {
            var subject = ConstructUnitSubject();

            Assert.AreEqual(0, subject.Level);
        }

        [Test]
        public void Ctor_Always_AddsBaseCharacteristicsConditionalHexOccupationBonus()
        {
            var subject = ConstructUnitSubject();

            Assert.True(subject.ConditionalHexOccupationBonuses.Contains(matchingConditionalHexOccupationBonus1));
            Assert.True(subject.ConditionalHexOccupationBonuses.Contains(matchingConditionalHexOccupationBonus2));
            Assert.True(subject.ConditionalHexOccupationBonuses.Contains(otherConditionalHexOccupationBonus));
        }

        [Test]
        public void Ctor_Always_AddsBaseCharacteristicsConditionalStructureOccupationBonus()
        {
            var subject = ConstructUnitSubject();

            Assert.True(subject.ConditionalStructureOccupationBonuses.Contains(matchingConditionalStructureOccupationBonus1));
            Assert.True(subject.ConditionalStructureOccupationBonuses.Contains(matchingConditionalStructureOccupationBonus2));
            Assert.True(subject.ConditionalStructureOccupationBonuses.Contains(wrongStructureConditionalStructureOccupationBonus));
            Assert.True(subject.ConditionalStructureOccupationBonuses.Contains(wrongUpgradeLevelConditionalStructureOccupationBonus));
        }

        [Test]
        public void LevelledTalents_AfterConstruction_ContainsAllAvailableTalentsAtLevelZero()
        {
            var subject = ConstructUnitSubject();

            Assert.True(subject.TalentsLevelled.ContainsKey(talent1));
            Assert.True(subject.TalentsLevelled.ContainsKey(talent2));
            Assert.AreEqual(0, subject.TalentsLevelled[talent1]);
            Assert.AreEqual(0, subject.TalentsLevelled[talent2]);
        }

        [Test]
        public void CurrentName_Always_ReturnsNameFromTemplate()
        {
            var subject = ConstructUnitSubject();

            Assert.AreEqual(unitName, subject.CurrentName);
        }

        [Test]
        public void UIText_Always_ReturnsDescriptionFromTemplate()
        {
            var subject = ConstructUnitSubject();

            Assert.AreEqual(unitDescription, subject.UIText);
        }

        [Test]
        public void Ctor_IfStartingHexContainsAnAuraThatAffectsUnits_AppliesAura()
        {
            var hexDataWithAura = Substitute.For<IHexData>();

            var aura = Substitute.For<IDefenderAura>();
            aura.DefenderEffectTemplate.Affects.Returns(DefenderEffectAffectsType.UNITS);
            aura.DefenderAuraTemplate.Affects.Returns(DefenderEffectAffectsType.UNITS);

            hexDataWithAura.DefenderAurasHere.Returns(new CallbackList<IDefenderAura> { aura });

            mapData.GetHexAt(startPosition).Returns(hexDataWithAura);

            var subject = ConstructUnitSubject();

            Assert.True(subject.AffectedByDefenderAuras.Contains(aura));
        }

        [Test]
        public void OnAuraAddedToHex_IfAuraAffectsUnits_AppliesAura()
        {
            var auraAttributeModifier = Substitute.For<INamedAttributeModifier<DeAttrName>>();
            var auraAttributeModifierList = new List<INamedAttributeModifier<DeAttrName>>
            {
                auraAttributeModifier
            };

            var aura = Substitute.For<IDefenderAura>();
            aura.DefenderEffectTemplate.Affects.Returns(DefenderEffectAffectsType.UNITS);
            aura.DefenderAuraTemplate.Affects.Returns(DefenderEffectAffectsType.UNITS);
            aura.DefenderEffectTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);
            aura.DefenderAuraTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);

            var subject = ConstructUnitSubject();

            aurasAtStartHex.Add(aura);

            Assert.True(subject.AffectedByDefenderAuras.Contains(aura));
            attributes.Received(1).AddModifier(auraAttributeModifier);
        }

        [Test]
        public void OnAuraRemovedFromHex_IfAuraAffectsUnits_RemovesAura()
        {
            var auraAttributeModifier = Substitute.For<INamedAttributeModifier<DeAttrName>>();
            var auraAttributeModifierList = new List<INamedAttributeModifier<DeAttrName>>
            {
                auraAttributeModifier
            };

            var aura = Substitute.For<IDefenderAura>();
            aura.DefenderEffectTemplate.Affects.Returns(DefenderEffectAffectsType.UNITS);
            aura.DefenderAuraTemplate.Affects.Returns(DefenderEffectAffectsType.UNITS);
            aura.DefenderEffectTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);
            aura.DefenderAuraTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);

            var subject = ConstructUnitSubject();

            aurasAtStartHex.Add(aura);

            aurasAtStartHex.TryRemove(aura);

            Assert.False(subject.AffectedByDefenderAuras.Contains(aura));
            attributes.Received(1).TryRemoveModifier(auraAttributeModifier);
        }

        [Test]
        public void FrameUpdate_InBuildMode_DoesNotChangeTimeActiveOrTimeIdle()
        {
            gameStateManager.CurrentGameMode.Returns(GameMode.BUILD);

            var subject = ConstructUnitSubject();

            frameUpdater.RunUpdate(0.3f);

            AssertExt.Approximately(0f, subject.TimeActive);
            AssertExt.Approximately(0f, subject.TimeIdle);
        }

        [Test]
        public void FrameUpdate_WhenNoAbilityExecuted_AddsDeltaTimeToTimeIdle()
        {
            var subject = ConstructUnitSubject();

            gameStateManager.CurrentGameMode.Returns(GameMode.DEFEND);
            gameStateManager.OnEnterDefendMode += Raise.EventWith<EAOnEnterDefendMode>();

            frameUpdater.RunUpdate(defaultDeltaTime);

            AssertExt.Approximately(defaultDeltaTime, subject.TimeIdle);
        }

        [Test]
        public void FrameUpdate_AfterAbilityExecutedAndBeforeOffCooldown_AddsDeltaTimeToTimeActive()
        {
            var subject = ConstructUnitSubject();

            gameStateManager.CurrentGameMode.Returns(GameMode.DEFEND);
            gameStateManager.OnEnterDefendMode += Raise.EventWith<EAOnEnterDefendMode>();

            var defendModeAbility = Substitute.For<IDefendModeAbility>();
            abilities.OnDefendModeAbilityAdded += Raise.EventWith(new EAOnDefendModeAbilityAdded(defendModeAbility));

            frameUpdater.RunUpdate(defaultDeltaTime);

            defendModeAbility.OnAbilityExecuted += Raise.EventWith(new EAOnAbilityExecuted(defendModeAbility));

            frameUpdater.RunUpdate(defaultDeltaTime);

            AssertExt.Approximately(defaultDeltaTime, subject.TimeActive);
        }

        [Test]
        public void FrameUpdate_AfterAbilityExecutedAndAllAbilitiesOffCooldownAgain_AddsDeltaTimeToTimeIdle()
        {
            var subject = ConstructUnitSubject();

            gameStateManager.CurrentGameMode.Returns(GameMode.DEFEND);
            gameStateManager.OnEnterDefendMode += Raise.EventWith<EAOnEnterDefendMode>();

            var defendModeAbility = Substitute.For<IDefendModeAbility>();
            abilities.OnDefendModeAbilityAdded += Raise.EventWith(new EAOnDefendModeAbilityAdded(defendModeAbility));

            frameUpdater.RunUpdate(defaultDeltaTime);

            defendModeAbility.OnAbilityExecuted += Raise.EventWith(new EAOnAbilityExecuted(defendModeAbility));

            frameUpdater.RunUpdate(defaultDeltaTime);

            abilities.DefendModeAbilityManager.OnAllDefendModeAbilitiesOffCooldown += Raise.EventWith(new EAOnAllDefendModeAbilitiesOffCooldown());

            frameUpdater.RunUpdate(defaultDeltaTime);

            AssertExt.Approximately(2*defaultDeltaTime, subject.TimeIdle);
        }

        [Test]
        public void OnEnterBuildMode_Always_ResetsTimeActiveAndTimeIdle()
        {
            var subject = ConstructUnitSubject();

            SimulateRound(
                timeIdle: defaultDeltaTime,
                timeActive: defaultDeltaTime,
                revertToBuildMode: false
            );

            frameUpdater.RunUpdate(defaultDeltaTime);

            gameStateManager.CurrentGameMode.Returns(GameMode.BUILD);
            gameStateManager.OnEnterBuildMode += Raise.EventWith<EAOnEnterBuildMode>();

            AssertExt.Approximately(0f, subject.TimeActive);
            AssertExt.Approximately(0f, subject.TimeIdle);
        }

        [Test]
        public void OnEnterBuildMode_Always_TriggersOnConditionalOccupationBonusesEventWithConditionalHexBonusProportionalToTimeIdle()
        {
            var subject = ConstructUnitSubject();

            var eventTester = new EventTester<EAOnTriggeredConditionalOccupationBonus>();
            subject.OnTriggeredConditionalOccupationBonuses += eventTester.Handler;

            var timeActiveForTest = 0.7f;
            var timeIdleForTest = 0.3f;
            var totalTimeForTest = timeActiveForTest + timeIdleForTest;

            SimulateRound(
                timeIdle: timeIdleForTest,
                timeActive: timeActiveForTest,
                revertToBuildMode: true
             );

            int expectedResource1Amount =
                Mathf.RoundToInt(
                    (matchingConditionalHexOccupationBonus1Resource1Amount + matchingConditionalHexOccupationBonus2Resource1Amount) * timeIdleForTest / totalTimeForTest
                );

            int expectedResource2Amount =
                Mathf.RoundToInt(
                    (matchingConditionalHexOccupationBonus1Resource2Amount + matchingConditionalHexOccupationBonus2Resource2Amount) * timeIdleForTest / totalTimeForTest
                );

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => 
            {
                return
                    args.HexTransaction.GetTransactionAmount(resource1) == expectedResource1Amount &&
                    args.HexTransaction.GetTransactionAmount(resource2) == expectedResource2Amount;
            });
        }

        [Test]
        public void OnEnterBuildMode_Always_TriggersOnConditionalOccupationBonusEventWithConditionalStructureBonusProportionalToTimeIdle()
        {
            var subject = ConstructUnitSubject();

            var eventTester = new EventTester<EAOnTriggeredConditionalOccupationBonus>();
            subject.OnTriggeredConditionalOccupationBonuses += eventTester.Handler;

            var timeActiveForTest = 0.7f;
            var timeIdleForTest = 0.3f;
            var totalTimeForTest = timeActiveForTest + timeIdleForTest;

            SimulateRound(
                timeIdle: timeIdleForTest,
                timeActive: timeActiveForTest,
                revertToBuildMode: true);

            int expectedResource1Amount =
                Mathf.RoundToInt(
                    (matchingConditionalStructureOccupationBonus1Resource1Amount + matchingConditionalStructureOccupationBonus2Resource1Amount) * timeIdleForTest / totalTimeForTest
                );

            int expectedResource2Amount =
                Mathf.RoundToInt(
                    (matchingConditionalStructureOccupationBonus1Resource2Amount + matchingConditionalStructureOccupationBonus2Resource2Amount) * timeIdleForTest / totalTimeForTest
                );

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args =>
            {
                return
                    args.StructureTransaction.GetTransactionAmount(resource1) == expectedResource1Amount &&
                    args.StructureTransaction.GetTransactionAmount(resource2) == expectedResource2Amount;
            });
        }

        [Test]
        public void OnEnterBuildMode_AfterOneRound_AddsExperienceProportionalToTimeActive()
        {
            var subject = ConstructUnitSubject();

            var timeActiveForTest = 0.7f;
            var timeIdleForTest = 0.3f;
            var totalTimeForTest = timeActiveForTest + timeIdleForTest;

            SimulateRound(
                timeActive: timeActiveForTest,
                timeIdle: timeIdleForTest,
                revertToBuildMode: true
            );

            Assert.AreEqual(Mathf.RoundToInt((timeActiveForTest / totalTimeForTest) * 100), subject.Experience);
        }

        [Test]
        public void OnEnterBuildMode_AfterOneRound_AddsFatigueProportionalToTimeActive()
        {
            var subject = ConstructUnitSubject();

            var timeActiveForTest = 0.7f;
            var timeIdleForTest = 0.3f;
            var totalTimeForTest = timeActiveForTest + timeIdleForTest;

            SimulateRound(
                timeIdle: timeIdleForTest,
                timeActive: timeActiveForTest,
                revertToBuildMode: true
            );

            Assert.AreEqual(Mathf.RoundToInt((timeActiveForTest / totalTimeForTest) * 10) - 5, subject.Fatigue);
        }

        [Test]
        public void OnEnterBuildMode_AfterSecondRound_AddsExperienceTakingIntoAccountTimeActiveAndFatigue()
        {
            var subject = ConstructUnitSubject();

            var timeActiveForTest = 0.9f;
            var timeIdleForTest = 0.1f;
            var totalTimeForTest = timeActiveForTest + timeIdleForTest;

            var defendModeAbility = Substitute.For<IDefendModeAbility>();
            abilities.OnDefendModeAbilityAdded += Raise.EventWith(new EAOnDefendModeAbilityAdded(defendModeAbility));

            SimulateRound(
                timeIdle: timeIdleForTest,
                timeActive: timeActiveForTest,
                revertToBuildMode: true,
                endWithAllAbilitiesOffCooldown: true
            );

            var expectedFirstRoundFatigue = Mathf.RoundToInt((timeActiveForTest / totalTimeForTest) * 10) - 5;

            SimulateRound(
                timeIdle: timeIdleForTest,
                timeActive: timeActiveForTest,
                revertToBuildMode: true
            );

            var expectedFatigueFactor = Mathf.Clamp(1 - (CustomMath.SignedOddRoot((expectedFirstRoundFatigue - fatigueInflectionPoint) / fatigueShallownessMultiplier, 3) + Mathf.Pow(fatigueInflectionPoint / fatigueShallownessMultiplier, 1f / 3f)), 0f, 1f);

            var expectedExperience = Mathf.RoundToInt((timeActiveForTest / totalTimeForTest) * 100) + Mathf.RoundToInt((timeActiveForTest / totalTimeForTest) * 100 * expectedFatigueFactor);

            Assert.AreEqual(expectedExperience, subject.Experience);
        }

        [Test]
        public void OnEnterBuildMode_AfterSecondRoundWhenLessThan50PercentActive_ReducesFatigue()
        {
            var subject = ConstructUnitSubject();

            var timeActiveForFirstRound = 0.9f;
            var timeIdleForFirstRound = 0.1f;
            var totalTimeForFirstRound = timeActiveForFirstRound + timeIdleForFirstRound;

            var timeActiveForSecondRound = 0.2f;
            var timeIdleForSecondRound = 0.8f;
            var totalTimeForSecondRound = timeActiveForSecondRound + timeIdleForSecondRound;

            SimulateRound(
                timeIdle: timeIdleForFirstRound,
                timeActive: timeActiveForFirstRound,
                revertToBuildMode: true
            );

            SimulateRound(
                timeIdle: timeIdleForSecondRound,
                timeActive: timeActiveForSecondRound,
                revertToBuildMode: true
            );

            var expectedFatigueAfterRound1 = Mathf.RoundToInt((timeActiveForFirstRound / totalTimeForFirstRound) * 10) - 5;
            var expectedFatigueAfterRound2 = expectedFatigueAfterRound1 + Mathf.RoundToInt((timeActiveForSecondRound / totalTimeForSecondRound) * 10) - 5;

            Assert.AreEqual(expectedFatigueAfterRound2, subject.Fatigue);
        }


        [Test]
        public void OnEnterBuildMode_Always_HasMinimumFatigueZero()
        {
            var subject = ConstructUnitSubject();

            var timeActiveForFirstRound = 0.6f;
            var timeIdleForFirstRound = 0.4f;

            var timeActiveForSecondRound = 0.1f;
            var timeIdleForSecondRound = 0.9f;

            var defendModeAbility = Substitute.For<IDefendModeAbility>();
            abilities.OnDefendModeAbilityAdded += Raise.EventWith(new EAOnDefendModeAbilityAdded(defendModeAbility));

            //Round 1
            SimulateRound(
                timeIdle: timeIdleForFirstRound,
                timeActive: timeActiveForFirstRound,
                revertToBuildMode: true
            );

            //Round 2
            SimulateRound(
                timeIdle: timeIdleForSecondRound,
                timeActive: timeActiveForSecondRound,
                revertToBuildMode: true
            );

            //Assert
            Assert.AreEqual(0, subject.Fatigue);
        }

        [Test]
        public void OnEnterBuildMode_Always_FiresOnExperienceFatigueLevelChangedEventWithCorrectValues()
        {
            var subject = ConstructUnitSubject();

            var eventTester = new EventTester<EAOnExperienceFatigueLevelChange>();
            subject.OnExperienceFatigueLevelChanged += eventTester.Handler;

            var timeActiveForTest = 0.6f;
            var timeIdleForTest = 0.4f;
            var totalTimeForTest = timeActiveForTest + timeIdleForTest;

            SimulateRound(
                timeIdle: timeIdleForTest,
                timeActive: timeActiveForTest,
                revertToBuildMode: true
            );

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => {
                return
                    args.NewExperienceValue == Mathf.RoundToInt((timeActiveForTest / totalTimeForTest) * 100) &&
                    args.NewFatigueValue == Mathf.RoundToInt((timeActiveForTest / totalTimeForTest) * 10) - 5;
            });
        }

        [Test]
        public void OnEnterBuildMode_IfEnoughTotalExperienceGainedToLevelUp_IncreasesLevelUpsPending()
        {
            var subject = ConstructUnitSubject();

            var timeActiveForTest = 0.9f;
            var timeIdleForTest = 0.1f;

            var defendModeAbility = Substitute.For<IDefendModeAbility>();
            abilities.OnDefendModeAbilityAdded += Raise.EventWith(new EAOnDefendModeAbilityAdded(defendModeAbility));

            SimulateRound(
                timeIdle: timeIdleForTest,
                timeActive: timeActiveForTest,
                revertToBuildMode: true
            );

            SimulateRound(
                timeIdle: timeIdleForTest,
                timeActive: timeActiveForTest,
                revertToBuildMode: true
            );

            //Assert
            Assert.AreEqual(1, subject.LevelUpsPending);
        }

        [Test]
        public void TryLevelUp_IfNotEnoughExperienceGainedYet_ReturnsFalse()
        {
            var subject = ConstructUnitSubject();

            var result = subject.TryLevelUp(talent1);

            Assert.False(result);
        }

        [Test]
        public void TryLevelUp_IfAlreadyLevelledTalentToMax_ReturnsFalse()
        {
            var experienceToLevelUp = 1;

            template.ExperienceToLevelUp.Returns(experienceToLevelUp);

            var subject = ConstructUnitSubject();

            SimulateRound(
                timeIdle: defaultDeltaTime,
                timeActive: defaultDeltaTime,
                revertToBuildMode: true
            );

            subject.TryLevelUp(talent1);
            
            subject.TryLevelUp(talent1);

            var result = subject.TryLevelUp(talent1);

            Assert.False(result);
        }

        [Test]
        public void TryLevelUp_IfLevelUpPendingAndTalentNotMaxed_IncrementsLevelledTalentsForThatTalent()
        {
            var experienceToLevelUp = 1;

            template.ExperienceToLevelUp.Returns(experienceToLevelUp);

            var subject = ConstructUnitSubject();

            SimulateRound(
                timeIdle: defaultDeltaTime,
                timeActive: defaultDeltaTime,
                revertToBuildMode: true
            );

            subject.TryLevelUp(talent1);

            Assert.AreEqual(1, subject.TalentsLevelled[talent1]);
        }

        [Test]
        public void TryLevelUp_IfLevelUpPendingAndTalentNotMaxed_IncreasesLevel()
        {
            var experienceToLevelUp = 1;

            template.ExperienceToLevelUp.Returns(experienceToLevelUp);

            var subject = ConstructUnitSubject();

            SimulateRound(
                timeIdle: defaultDeltaTime,
                timeActive: defaultDeltaTime,
                revertToBuildMode: true
            );

            subject.TryLevelUp(talent1);

            Assert.AreEqual(1, subject.Level);
        }

        [Test]
        public void TryLevelUp_IfLevelUpPendingAndTalentNotMaxed_DecreasesLevelUpsPending()
        {
            var experienceToLevelUp = 1;

            template.ExperienceToLevelUp.Returns(experienceToLevelUp);

            var subject = ConstructUnitSubject();

            SimulateRound(
                timeIdle: defaultDeltaTime,
                timeActive: defaultDeltaTime,
                revertToBuildMode: true
            );

            var levelUpsPendingPreLevelUp = subject.LevelUpsPending;

            subject.TryLevelUp(talent1);

            Assert.AreEqual(levelUpsPendingPreLevelUp - 1, subject.LevelUpsPending);
        }

        [Test]
        public void TryLevelUp_IfLevelUpPendingAndTalentNotMaxed_ReturnsTrue()
        {
            var experienceToLevelUp = 1;

            template.ExperienceToLevelUp.Returns(experienceToLevelUp);

            var subject = ConstructUnitSubject();

            SimulateRound(
                timeIdle: defaultDeltaTime,
                timeActive: defaultDeltaTime,
                revertToBuildMode: true
            );

            var result = subject.TryLevelUp(talent1);

            Assert.True(result);
        }

        [Test]
        public void TryLevelUp_IfLevelUpPendingAndTalentNotMaxed_FiresOnExperienceFatigueLevelChangedEvent()
        {
            var experienceToLevelUp = 1;

            template.ExperienceToLevelUp.Returns(experienceToLevelUp);

            var subject = ConstructUnitSubject();

            SimulateRound(
                timeIdle: defaultDeltaTime,
                timeActive: defaultDeltaTime,
                revertToBuildMode: true
            );

            var eventTester = new EventTester<EAOnExperienceFatigueLevelChange>();
            subject.OnExperienceFatigueLevelChanged += eventTester.Handler;

            subject.TryLevelUp(talent1);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.NewLevel == 1);
        }

        [Test]
        public void TryLevelUp_IfTalentNotLevelledUpYet_AppliesTheImprovementForTheFirstLevel()
        {
            var experienceToLevelUp = 1;

            template.ExperienceToLevelUp.Returns(experienceToLevelUp);

            var subject = ConstructUnitSubject();

            var eventTester = new EventTester<EAOnExperienceFatigueLevelChange>();
            subject.OnExperienceFatigueLevelChanged += eventTester.Handler;

            SimulateRound(
                timeIdle: defaultDeltaTime,
                timeActive: defaultDeltaTime,
                revertToBuildMode: true
            );

            subject.TryLevelUp(talent1);

            abilities.Received(1).AddAbility(talent1Level1Ability);
            attributes.Received(1).AddModifier(talent1Level1AttributeModifier);
            Assert.True(subject.FlatHexOccupationBonuses.Contains(talent1Level1FlatHexOccupationBonus));
            Assert.True(subject.ConditionalHexOccupationBonuses.Contains(talent1Level1ConditionalHexOccupationBonus));
            Assert.True(subject.ConditionalStructureOccupationBonuses.Contains(talent1Level1ConditionalStructureOccupationBonus));
            Assert.True(subject.AurasEmitted.Contains(talent1Level1DefenderAura));
        }

        [Test]
        public void TryLevelUp_IfTalentAlreadyLevelledUp_RemovedPreviousLevelImprovementAndAppliesNewLevelImprovement()
        {
            var experienceToLevelUp = 1;

            template.ExperienceToLevelUp.Returns(experienceToLevelUp);

            var subject = ConstructUnitSubject();

            var eventTester = new EventTester<EAOnExperienceFatigueLevelChange>();
            subject.OnExperienceFatigueLevelChanged += eventTester.Handler;

            SimulateRound(
                timeIdle: defaultDeltaTime,
                timeActive: defaultDeltaTime,
                revertToBuildMode: true
            );

            subject.TryLevelUp(talent1);
            subject.TryLevelUp(talent1);

            abilities.Received(1).AddAbility(talent1Level2Ability);

            abilities.Received(1).TryRemoveAbility(talent1Level1AbilityTemplate);
            attributes.Received(1).TryRemoveModifier(talent1Level1AttributeModifier);
            Assert.False(subject.FlatHexOccupationBonuses.Contains(talent1Level1FlatHexOccupationBonus));
            Assert.False(subject.ConditionalHexOccupationBonuses.Contains(talent1Level1ConditionalHexOccupationBonus));
            Assert.False(subject.ConditionalStructureOccupationBonuses.Contains(talent1Level1ConditionalStructureOccupationBonus));
            Assert.False(subject.AurasEmitted.Contains(talent1Level1DefenderAura));
        }

        [Test]
        public void Move_Always_ChangesPositionToTargetCoord()
        {
            var subject = ConstructUnitSubject();

            subject.Move(movementTarget);

            Assert.AreEqual(movementTarget, subject.CoordPosition);
        }

        [Test]
        public void Move_Always_FiresOnMovedEvent()
        {
            var subject = ConstructUnitSubject();

            var eventTester = new EventTester<EAOnMoved>();
            subject.OnMoved += eventTester.Handler;

            subject.Move(movementTarget);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.ToPosition == movementTarget);
        }

        [Test]
        public void Move_IfUnitHasEmittedAuras_ClearsAuras()
        {
            var subject = ConstructUnitSubject();

            subject.Move(movementTarget);

            defenderAura.Received(1).ClearAura();
        }

        [Test]
        public void Move_IfUnitHasEmittedAura_InitialisesAurasAtNewPosition()
        {
            var subject = ConstructUnitSubject();

            subject.Move(movementTarget);

            movedToHexData.Received(1).AddDefenderAura(defenderAura);
        }

        [Test]
        public void Move_IfUnitIsAffectedByAurasThatAreNotAtNewHex_RemovesThoseAuras()
        {
            var auraAttributeModifier = Substitute.For<INamedAttributeModifier<DeAttrName>>();
            var auraAttributeModifierList = new List<INamedAttributeModifier<DeAttrName>>
            {
                auraAttributeModifier
            };

            var aura = Substitute.For<IDefenderAura>();
            aura.DefenderAuraTemplate.Affects.Returns(DefenderEffectAffectsType.BOTH);
            aura.DefenderEffectTemplate.Affects.Returns(DefenderEffectAffectsType.BOTH);
            aura.DefenderAuraTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);
            aura.DefenderEffectTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);

            var subject = ConstructUnitSubject();

            aurasAtStartHex.Add(aura);

            subject.Move(movementTarget);

            Assert.False(subject.AffectedByDefenderAuras.Contains(aura));
            attributes.Received(1).TryRemoveModifier(auraAttributeModifier);
        }

        [Test]
        public void Move_IfNewPositionHasAuraThatAffectBoth_AppliesAura()
        {
            var auraAttributeModifier = Substitute.For<INamedAttributeModifier<DeAttrName>>();
            var auraAttributeModifierList = new List<INamedAttributeModifier<DeAttrName>>
            {
                auraAttributeModifier
            };

            var aura = Substitute.For<IDefenderAura>();
            aura.DefenderAuraTemplate.Affects.Returns(DefenderEffectAffectsType.BOTH);
            aura.DefenderEffectTemplate.Affects.Returns(DefenderEffectAffectsType.BOTH);
            aura.DefenderAuraTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);
            aura.DefenderEffectTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);

            movedToHexData.DefenderAurasHere.Returns(new CallbackList<IDefenderAura> { aura });

            var subject = ConstructUnitSubject();

            subject.Move(movementTarget);

            Assert.True(subject.AffectedByDefenderAuras.Contains(aura));
            attributes.Received(1).AddModifier(auraAttributeModifier);
        }

        [Test]
        public void Move_IfNewPositionHasAuraThatAffectsUnits_AppliesAura()
        {
            var auraAttributeModifier = Substitute.For<INamedAttributeModifier<DeAttrName>>();
            var auraAttributeModifierList = new List<INamedAttributeModifier<DeAttrName>>
            {
                auraAttributeModifier
            };

            var aura = Substitute.For<IDefenderAura>();
            aura.DefenderAuraTemplate.Affects.Returns(DefenderEffectAffectsType.UNITS);
            aura.DefenderEffectTemplate.Affects.Returns(DefenderEffectAffectsType.UNITS);
            aura.DefenderAuraTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);
            aura.DefenderEffectTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);

            movedToHexData.DefenderAurasHere.Returns(new CallbackList<IDefenderAura> { aura });

            var subject = ConstructUnitSubject();

            subject.Move(movementTarget);

            Assert.True(subject.AffectedByDefenderAuras.Contains(aura));
            attributes.Received(1).AddModifier(auraAttributeModifier);
        }

        [Test]
        public void OnRelevantDefenderAuraAddedToOldPosition_AfterMove_DoesNotApplyAura()
        {
            var auraAttributeModifier = Substitute.For<INamedAttributeModifier<DeAttrName>>();
            var auraAttributeModifierList = new List<INamedAttributeModifier<DeAttrName>>
            {
                auraAttributeModifier
            };

            var aura = Substitute.For<IDefenderAura>();
            aura.DefenderAuraTemplate.Affects.Returns(DefenderEffectAffectsType.BOTH);
            aura.DefenderEffectTemplate.Affects.Returns(DefenderEffectAffectsType.BOTH);
            aura.DefenderAuraTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);
            aura.DefenderEffectTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);

            var subject = ConstructUnitSubject();

            subject.Move(movementTarget);

            aurasAtStartHex.Add(aura);

            Assert.False(subject.AffectedByDefenderAuras.Contains(aura));
            attributes.Received(0).AddModifier(auraAttributeModifier);
        }

        [Test]
        public void OnRelevantDefenderAuraAddedToNewPosition_AfterMove_AppliesAura()
        {
            var auraAttributeModifier = Substitute.For<INamedAttributeModifier<DeAttrName>>();
            var auraAttributeModifierList = new List<INamedAttributeModifier<DeAttrName>>
            {
                auraAttributeModifier
            };

            var aura = Substitute.For<IDefenderAura>();
            aura.DefenderAuraTemplate.Affects.Returns(DefenderEffectAffectsType.BOTH);
            aura.DefenderEffectTemplate.Affects.Returns(DefenderEffectAffectsType.BOTH);
            aura.DefenderAuraTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);
            aura.DefenderEffectTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);

            var subject = ConstructUnitSubject();

            var aurasAtMoveToHex = new CallbackList<IDefenderAura>();
            movedToHexData.DefenderAurasHere.Returns(aurasAtMoveToHex);

            subject.Move(movementTarget);

            aurasAtMoveToHex.Add(aura);

            Assert.True(subject.AffectedByDefenderAuras.Contains(aura));
            attributes.Received(1).AddModifier(auraAttributeModifier);
        }

        [Test]
        public void RegenerateCachedDisallowedMovementDestinations_Always_GetsDisallowedMovementDestinationsFromMapForCurrentPosition()
        {
            var disallowedList = new List<Coord>();

            mapData.GetDisallowedMovementDestinationCoords(startPosition).Returns(disallowedList);

            var subject = ConstructUnitSubject();

            subject.RegenerateCachedDisallowedMovementDestinations();

            Assert.AreEqual(disallowedList, subject.CachedDisallowedMovementDestinations);
        }
    }
}