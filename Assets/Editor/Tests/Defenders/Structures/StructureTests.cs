using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Defenders;
using GrimoireTD.Defenders.Structures;
using GrimoireTD.Defenders.DefenderEffects;
using GrimoireTD.Map;
using GrimoireTD.Attributes;
using GrimoireTD.Economy;
using GrimoireTD.Abilities;

namespace GrimoireTD.Tests.StructureTests
{
    public class StructureTests : DefenderTests.DefenderTests
    {
        //Primitives and Basic Objects
        private string startingName = "Structure Starting Name";
        private string startingDescription = "Structure Starting Description";

        private string upgradedName = "Structure Upgraded Name";
        private string upgradedDescription = "Structure Upgraded Description";

        //Template Deps
        private IStructureTemplate template = Substitute.For<IStructureTemplate>();

        private IDefenderImprovement baseCharacteristics = Substitute.For<IDefenderImprovement>();

        private IStructureUpgrade upgrade1 = Substitute.For<IStructureUpgrade>();
        private IStructureUpgrade upgrade2 = Substitute.For<IStructureUpgrade>();

        private IStructureEnhancement upgrade1Enhancement1 = Substitute.For<IStructureEnhancement>();
        private IStructureEnhancement upgrade1Enhancement2 = Substitute.For<IStructureEnhancement>();

        private IStructureEnhancement upgrade2Enhancement1 = Substitute.For<IStructureEnhancement>();
        private IStructureEnhancement upgrade2Enhancement2 = Substitute.For<IStructureEnhancement>();

        private INamedAttributeModifier<DeAttrName> upgradeAttributeModifier = Substitute.For<INamedAttributeModifier<DeAttrName>>();

        private IAbilityTemplate upgradeAbilityTemplate = Substitute.For<IAbilityTemplate>();
        private IAbility upgradeAbility = Substitute.For<IAbility>();

        [OneTimeSetUp]
        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();

            //Template Deps
            template.BaseCharacteristics.Returns(baseCharacteristics);

            SetUpBaseCharacteristics(baseCharacteristics);

            template.StartingNameInGame.Returns(startingName);
            template.StartingDescription.Returns(startingDescription);

            template.StructureUpgrades.Returns(new List<IStructureUpgrade>
            {
                upgrade1,
                upgrade2
            });

            upgrade1.NewStructureName.Returns(upgradedName);
            upgrade1.NewStructureDescription.Returns(upgradedDescription);

            upgrade2.NewStructureName.Returns(upgradedName);
            upgrade2.NewStructureDescription.Returns(upgradedDescription);

            upgrade1.OptionalEnhancements.Returns(new List<IStructureEnhancement>
            {
                upgrade1Enhancement1,
                upgrade1Enhancement2
            });

            upgrade2.OptionalEnhancements.Returns(new List<IStructureEnhancement>
            {
                upgrade2Enhancement1,
                upgrade2Enhancement2
            });

            upgrade1.MainUpgradeBonus.AttributeModifiers.Returns(new List<INamedAttributeModifier<DeAttrName>>
            {
                upgradeAttributeModifier
            });

            upgrade1.MainUpgradeBonus.Abilities.Returns(new List<IAbilityTemplate>
            {
                upgradeAbilityTemplate
            });

            upgradeAbilityTemplate.GenerateAbility(Arg.Any<IDefender>()).Returns(upgradeAbility);

            upgrade1Enhancement1.EnhancementBonus.FlatHexOccupationBonuses.Returns(new List<IHexOccupationBonus>());

            upgrade1Enhancement1.EnhancementBonus.Auras.Returns(new List<IDefenderAuraTemplate>());
        }

        [SetUp]
        public override void EachTestSetUp()
        {
            base.EachTestSetUp();

            upgrade1Enhancement1.Cost.TransactionDictionary.Returns(new Dictionary<IReadOnlyResource, IResourceTransaction>());
            upgrade1Enhancement2.Cost.TransactionDictionary.Returns(new Dictionary<IReadOnlyResource, IResourceTransaction>());
            upgrade2Enhancement1.Cost.TransactionDictionary.Returns(new Dictionary<IReadOnlyResource, IResourceTransaction>());
            upgrade2Enhancement2.Cost.TransactionDictionary.Returns(new Dictionary<IReadOnlyResource, IResourceTransaction>());
        }

        protected override CDefender ConstructSubject()
        {
            return new CStructure(template, startPosition);
        }

        private CStructure ConstructStructureSubject()
        {
            return ConstructSubject() as CStructure;
        }

        [Test]
        public void UpgradesBought_AfterConstruction_ContainsAllAvailableUpgradesSetToFalse()
        {
            var subject = ConstructStructureSubject();

            Assert.True(subject.UpgradesBought.ContainsKey(upgrade1));
            Assert.True(subject.UpgradesBought.ContainsKey(upgrade2));
            Assert.False(subject.UpgradesBought[upgrade1]);
            Assert.False(subject.UpgradesBought[upgrade2]);
        }

        [Test]
        public void EnhancementsChosen_AfterConstruction_ContainsAllAvailabelEnhancementsSetToFalse()
        {
            var subject = ConstructStructureSubject();

            Assert.True(subject.EnhancementsChosen.ContainsKey(upgrade1Enhancement1));
            Assert.True(subject.EnhancementsChosen.ContainsKey(upgrade1Enhancement2));
            Assert.True(subject.EnhancementsChosen.ContainsKey(upgrade2Enhancement1));
            Assert.True(subject.EnhancementsChosen.ContainsKey(upgrade2Enhancement2));
            Assert.False(subject.EnhancementsChosen[upgrade1Enhancement1]);
            Assert.False(subject.EnhancementsChosen[upgrade1Enhancement2]);
            Assert.False(subject.EnhancementsChosen[upgrade2Enhancement1]);
            Assert.False(subject.EnhancementsChosen[upgrade2Enhancement2]);
        }

        [Test]
        public void CurrentName_AfterConstruction_IsStartingName()
        {
            var subject = ConstructStructureSubject();

            Assert.AreEqual(startingName, subject.CurrentName);
        }

        [Test]
        public void CurrentDescription_AfterConstruction_IsStartingDescription()
        {
            var subject = ConstructStructureSubject();

            Assert.AreEqual(startingDescription, subject.CurrentDescription);
        }

        [Test]
        public void Ctor_IfStartingHexContainsAnAuraThatAffectsStructures_AppliesAura()
        {
            var hexDataWithAura = Substitute.For<IHexData>();

            var auraAttributeModifier = Substitute.For<INamedAttributeModifier<DeAttrName>>();
            var auraAttributeModifierList = new List<INamedAttributeModifier<DeAttrName>>
            {
                auraAttributeModifier
            };

            var aura = Substitute.For<IDefenderAura>();
            aura.DefenderEffectTemplate.Affects.Returns(DefenderEffectAffectsType.STRUCTURES);
            aura.DefenderAuraTemplate.Affects.Returns(DefenderEffectAffectsType.STRUCTURES);
            aura.DefenderEffectTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);
            aura.DefenderAuraTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);

            hexDataWithAura.DefenderAurasHere.Returns(new CallbackList<IDefenderAura> { aura });

            mapData.GetHexAt(startPosition).Returns(hexDataWithAura);

            var subject = ConstructStructureSubject();

            Assert.True(subject.AffectedByDefenderAuras.Contains(aura));
            attributes.Received(1).AddModifier(auraAttributeModifier);
        }

        [Test]
        public void OnAuraAddedToHex_IfAuraAffectsStructures_AppliesAura()
        {
            var auraAttributeModifier = Substitute.For<INamedAttributeModifier<DeAttrName>>();
            var auraAttributeModifierList = new List<INamedAttributeModifier<DeAttrName>>
            {
                auraAttributeModifier
            };

            var aura = Substitute.For<IDefenderAura>();
            aura.DefenderEffectTemplate.Affects.Returns(DefenderEffectAffectsType.STRUCTURES);
            aura.DefenderAuraTemplate.Affects.Returns(DefenderEffectAffectsType.STRUCTURES);
            aura.DefenderEffectTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);
            aura.DefenderAuraTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);

            var subject = ConstructStructureSubject();

            aurasAtStartHex.Add(aura);

            Assert.True(subject.AffectedByDefenderAuras.Contains(aura));
            attributes.Received(1).AddModifier(auraAttributeModifier);
        }

        [Test]
        public void OnAuraRemovedFromHex_IfAuraAffectsStructures_RemovesAura()
        {
            var auraAttributeModifier = Substitute.For<INamedAttributeModifier<DeAttrName>>();
            var auraAttributeModifierList = new List<INamedAttributeModifier<DeAttrName>>
            {
                auraAttributeModifier
            };

            var aura = Substitute.For<IDefenderAura>();
            aura.DefenderEffectTemplate.Affects.Returns(DefenderEffectAffectsType.STRUCTURES);
            aura.DefenderAuraTemplate.Affects.Returns(DefenderEffectAffectsType.STRUCTURES);
            aura.DefenderEffectTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);
            aura.DefenderAuraTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);

            var subject = ConstructStructureSubject();

            aurasAtStartHex.Add(aura);

            aurasAtStartHex.TryRemove(aura);

            Assert.False(subject.AffectedByDefenderAuras.Contains(aura));
            attributes.Received(1).TryRemoveModifier(auraAttributeModifier);
        }

        [Test]
        public void Ctor_IfHexContainsAnAuraThatAffectsUnits_DoesNotApplyAura()
        {
            var hexDataWithAura = Substitute.For<IHexData>();

            var aura = Substitute.For<IDefenderAura>();
            aura.DefenderEffectTemplate.Affects.Returns(DefenderEffectAffectsType.UNITS);
            aura.DefenderAuraTemplate.Affects.Returns(DefenderEffectAffectsType.UNITS);

            hexDataWithAura.DefenderAurasHere.Returns(new CallbackList<IDefenderAura> { aura });

            mapData.GetHexAt(startPosition).Returns(hexDataWithAura);

            var subject = ConstructStructureSubject();

            Assert.False(subject.AffectedByDefenderAuras.Contains(aura));
        }

        [Test]
        public void CurrentUpgradeLevel_AfterConstruction_ReturnsNull()
        {
            var subject = ConstructStructureSubject();

            Assert.IsNull(subject.CurrentUpgradeLevel());
        }

        [Test]
        public void TryUpgrade_IfUpgradeAlreadyBought_ReturnsFalse()
        {
            var subject = ConstructStructureSubject();

            subject.TryUpgrade(upgrade1, upgrade1Enhancement1);

            var result = subject.TryUpgrade(upgrade1, upgrade1Enhancement2); ;

            Assert.False(result);
        }

        [Test]
        public void TryUpgrade_IfCantDoTransaction_ReturnsFalse()
        {
            var subject = ConstructStructureSubject();

            var resource = Substitute.For<IResource>();

            resource.CanDoTransaction(Arg.Any<int>()).Returns(false);

            upgrade1Enhancement1.Cost.TransactionDictionary.Returns(new Dictionary<IReadOnlyResource, IResourceTransaction>
            {
                { resource, new CResourceTransaction(resource, 1) }
            });

            var result = subject.TryUpgrade(upgrade1, upgrade1Enhancement1);

            Assert.False(result);
        }

        [Test]
        public void TryUpgrade_IfNotAlreadyBoughtAndCanDoTransaction_AppliesCombinedImprovement()
        {
            var subject = ConstructStructureSubject();

            subject.TryUpgrade(upgrade1, upgrade1Enhancement1);

            abilities.Received(1).AddAbility(upgradeAbility);
            attributes.Received(1).AddModifier(upgradeAttributeModifier);
        }

        [Test]
        public void TryUpgrade_IfNotAlreadyBoughtAndCanDoTransaction_SetsUpgradeToBoughtAndEnhancementToChosen()
        {
            var subject = ConstructStructureSubject();

            subject.TryUpgrade(upgrade1, upgrade1Enhancement1);

            Assert.True(subject.UpgradesBought[upgrade1]);
            Assert.True(subject.EnhancementsChosen[upgrade1Enhancement1]);
        }

        [Test]
        public void TryUpgrade_IfNotAlreadyBoughtAndCanDoTransaction_FiresOnUpgradedEvent()
        {
            var subject = ConstructStructureSubject();

            var eventTester = new EventTester<EAOnUpgraded>();
            subject.OnUpgraded += eventTester.Handler;

            subject.TryUpgrade(upgrade1, upgrade1Enhancement1);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.Upgrade == upgrade1 && args.Enhancement == upgrade1Enhancement1);
        }

        [Test]
        public void TryUpgrade_IfNotAlreadyBoughtAndCanDoTransaction_ChangesNameAndDescription()
        {
            var subject = ConstructStructureSubject();

            subject.TryUpgrade(upgrade1, upgrade1Enhancement1);

            Assert.AreEqual(upgradedName, subject.CurrentName);
            Assert.AreEqual(upgradedDescription, subject.CurrentDescription);
        }

        [Test]
        public void TryUpgrade_IfNotAlreadyBoughtAndCanDoTransaction_ReturnsTrue()
        {
            var subject = ConstructStructureSubject();

            var result = subject.TryUpgrade(upgrade1, upgrade1Enhancement1);

            Assert.True(result);
        }
    }
}