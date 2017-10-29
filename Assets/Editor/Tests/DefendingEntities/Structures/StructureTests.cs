using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.DefendingEntities;
using GrimoireTD.DefendingEntities.Structures;
using GrimoireTD.DefendingEntities.DefenderEffects;
using GrimoireTD.Map;
using GrimoireTD.Attributes;

namespace GrimoireTD.Tests.StructureTests
{
    public class StructureTests : DefendingEntityTests.DefendingEntityTests
    {
        //Primitives and Basic Objects
        private string startingName = "Structure Starting Name";

        private string startingDescription = "Structure Starting Description";

        //Model and Frame Updater


        //Instance Dependency Provider Deps


        //Template Deps
        private IStructureTemplate template = Substitute.For<IStructureTemplate>();

        private IDefendingEntityImprovement baseCharacteristics = Substitute.For<IDefendingEntityImprovement>();

        private IStructureUpgrade upgrade1 = Substitute.For<IStructureUpgrade>();
        private IStructureUpgrade upgrade2 = Substitute.For<IStructureUpgrade>();

        private IStructureEnhancement upgrade1Enhancement1 = Substitute.For<IStructureEnhancement>();
        private IStructureEnhancement upgrade1Enhancement2 = Substitute.For<IStructureEnhancement>();

        private IStructureEnhancement upgrade2Enhancement1 = Substitute.For<IStructureEnhancement>();
        private IStructureEnhancement upgrade2Enhancement2 = Substitute.For<IStructureEnhancement>();

        //Other Deps Passed To Ctor or SetUp


        //Other Objects Passed To Methods


        [OneTimeSetUp]
        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();

            //Model and Frame Updater
            mapData.CoordsInRange(defenderAuraRange, startPosition).Returns(new List<Coord> { startPosition });

            //Instance Dependency Provider Deps

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


            //Other Deps Passed To Ctor or SetUp


            //Other Objects Passed To Methods


        }

        [SetUp]
        public override void EachTestSetUp()
        {
            base.EachTestSetUp();

            upgrade1Enhancement1.Cost.CanDoTransaction().Returns(true);
            upgrade1Enhancement2.Cost.CanDoTransaction().Returns(true);
            upgrade2Enhancement1.Cost.CanDoTransaction().Returns(true);
            upgrade2Enhancement2.Cost.CanDoTransaction().Returns(true);
        }

        protected override CDefendingEntity ConstructSubject()
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

            var auraAttributeModifier = Substitute.For<INamedAttributeModifier<DEAttrName>>();
            var auraAttributeModifierList = new List<INamedAttributeModifier<DEAttrName>>
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
            var auraAttributeModifier = Substitute.For<INamedAttributeModifier<DEAttrName>>();
            var auraAttributeModifierList = new List<INamedAttributeModifier<DEAttrName>>
            {
                auraAttributeModifier
            };

            var aura = Substitute.For<IDefenderAura>();
            aura.DefenderEffectTemplate.Affects.Returns(DefenderEffectAffectsType.STRUCTURES);
            aura.DefenderAuraTemplate.Affects.Returns(DefenderEffectAffectsType.STRUCTURES);
            aura.DefenderEffectTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);
            aura.DefenderAuraTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);

            var subject = ConstructSubject();

            aurasAtStartHex.Add(aura);

            Assert.True(subject.AffectedByDefenderAuras.Contains(aura));
            attributes.Received(1).AddModifier(auraAttributeModifier);
        }

        [Test]
        public void OnAuraRemovedFromHex_IfAuraAffectsStructures_RemovesAura()
        {
            var auraAttributeModifier = Substitute.For<INamedAttributeModifier<DEAttrName>>();
            var auraAttributeModifierList = new List<INamedAttributeModifier<DEAttrName>>
            {
                auraAttributeModifier
            };

            var aura = Substitute.For<IDefenderAura>();
            aura.DefenderEffectTemplate.Affects.Returns(DefenderEffectAffectsType.STRUCTURES);
            aura.DefenderAuraTemplate.Affects.Returns(DefenderEffectAffectsType.STRUCTURES);
            aura.DefenderEffectTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);
            aura.DefenderAuraTemplate.Improvement.AttributeModifiers.Returns(auraAttributeModifierList);

            var subject = ConstructSubject();

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

        }

        [Test]
        public void TryUpgrade_IfCantDoTransaction_ReturnsFalse()
        {

        }

        [Test]
        public void TryUpgrade_IfNotAlreadyBoughtAndCanDoTransaction_AppliesCombinedImprovement()
        {

        }

        [Test]
        public void TryUpgrade_IfNotAlreadyBoughtAndCanDoTransaction_SetsUpgradeToBoughtAndEnhancementToChosen()
        {

        }

        [Test]
        public void TryUpgrade_IfNotAlreadyBoughtAndCanDoTransaction_FiresOnUpgradedEvent()
        {

        }

        [Test]
        public void TryUpgrade_IfNotAlreadyBoughtAndCanDoTransaction_ChangesNameAndDescription()
        {

        }

        [Test]
        public void TryUpgrade_IfNotAlreadyBoughtAndCanDoTransaction_ReturnsTrue()
        {

        }
    }
}