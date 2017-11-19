using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Attributes;
using GrimoireTD.Economy;
using GrimoireTD.Defenders;
using GrimoireTD.Abilities;
using GrimoireTD.Defenders.DefenderEffects;
using GrimoireTD.Defenders.Units;

namespace GrimoireTD.Tests.DefenderImprovementTests
{
    public class DefenderImprovementTests
    {
        //Other Deps Passed To Ctor or SetUp
        protected INamedAttributeModifier<DeAttrName> attributeModifier = Substitute.For<INamedAttributeModifier<DeAttrName>>();

        protected IHexOccupationBonus flatHexOccupationBonus = Substitute.For<IHexOccupationBonus>();

        protected IAbilityTemplate abilityTemplate = Substitute.For<IAbilityTemplate>();

        protected IDefenderAuraTemplate auraTemplate = Substitute.For<IDefenderAuraTemplate>();

        //Other Objects Passed To Methods
        protected IDefenderImprovement otherDefenderImprovement = Substitute.For<IDefenderImprovement>();

        protected INamedAttributeModifier<DeAttrName> otherImprovementAttributeModifier = Substitute.For<INamedAttributeModifier<DeAttrName>>();
        protected IHexOccupationBonus otherImprovementFlatHexOccupationBonus = Substitute.For<IHexOccupationBonus>();
        protected IAbilityTemplate otherImprovementAbilityTemplate = Substitute.For<IAbilityTemplate>();
        protected IDefenderAuraTemplate otherImprovementAuraTemplate = Substitute.For<IDefenderAuraTemplate>();

        protected IUnitImprovement otherUnitImprovement = Substitute.For<IUnitImprovement>();

        protected IHexOccupationBonus otherUnitImprovementConditionalHexOccupationBonus = Substitute.For<IHexOccupationBonus>();
        protected IStructureOccupationBonus otherUnitImprovementConditionalStructureOccupationBonus = Substitute.For<IStructureOccupationBonus>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //Other Objects Passed To Methods
            otherDefenderImprovement.AttributeModifiers.Returns(new List<INamedAttributeModifier<DeAttrName>> { otherImprovementAttributeModifier });
            otherDefenderImprovement.FlatHexOccupationBonuses.Returns(new List<IHexOccupationBonus> { otherImprovementFlatHexOccupationBonus });
            otherDefenderImprovement.Abilities.Returns(new List<IAbilityTemplate> { otherImprovementAbilityTemplate });
            otherDefenderImprovement.Auras.Returns(new List<IDefenderAuraTemplate> { otherImprovementAuraTemplate });

            otherUnitImprovement.AttributeModifiers.Returns(new List<INamedAttributeModifier<DeAttrName>> { otherImprovementAttributeModifier });
            otherUnitImprovement.FlatHexOccupationBonuses.Returns(new List<IHexOccupationBonus> { otherImprovementFlatHexOccupationBonus });
            otherUnitImprovement.Abilities.Returns(new List<IAbilityTemplate> { otherImprovementAbilityTemplate });
            otherUnitImprovement.Auras.Returns(new List<IDefenderAuraTemplate> { otherImprovementAuraTemplate });
            otherUnitImprovement.ConditionalHexOccupationBonuses.Returns(new List<IHexOccupationBonus> { otherUnitImprovementConditionalHexOccupationBonus });
            otherUnitImprovement.ConditionalStructureOccupationBonuses.Returns(new List<IStructureOccupationBonus> { otherUnitImprovementConditionalStructureOccupationBonus });
        }

        protected virtual CDefenderImprovement ConstructSubject()
        {
            return new CDefenderImprovement(
                new List<INamedAttributeModifier<DeAttrName>> { attributeModifier },
                new List<IHexOccupationBonus> { flatHexOccupationBonus },
                new List<IAbilityTemplate> { abilityTemplate },
                new List<IDefenderAuraTemplate> { auraTemplate }
            );
        }

        [Test]
        public void Ctor_Always_AddsAttrbuteModifiers()
        {
            var subject = ConstructSubject();

            Assert.True(subject.AttributeModifiers.Contains(attributeModifier));
        }

        [Test]
        public void Ctor_Always_AddsFlatHexOccupationBonuses()
        {
            var subject = ConstructSubject();

            Assert.True(subject.FlatHexOccupationBonuses.Contains(flatHexOccupationBonus));
        }

        [Test]
        public void Ctor_Always_AddsAbilityTemplates()
        {
            var subject = ConstructSubject();

            Assert.True(subject.Abilities.Contains(abilityTemplate));
        }

        [Test]
        public void Ctor_Always_AddsDefenderAuraTemplates()
        {
            var subject = ConstructSubject();

            Assert.True(subject.Auras.Contains(auraTemplate));
        }

        [Test]
        public void CombineWith_Always_CombinesAttributeModifiers()
        {
            var subject = ConstructSubject();

            var result = subject.CombineWith(otherDefenderImprovement);

            Assert.True(result.AttributeModifiers.Contains(attributeModifier));
            Assert.True(result.AttributeModifiers.Contains(otherImprovementAttributeModifier));
        }

        [Test]
        public void CombineWith_Always_CombinesFlatHexOccupationBonuses()
        {
            var subject = ConstructSubject();

            var result = subject.CombineWith(otherDefenderImprovement);

            Assert.True(result.AttributeModifiers.Contains(attributeModifier));
            Assert.True(result.AttributeModifiers.Contains(otherImprovementAttributeModifier));
        }

        [Test]
        public void CombineWith_Always_CombinesAbilityTemplates()
        {
            var subject = ConstructSubject();

            var result = subject.CombineWith(otherDefenderImprovement);

            Assert.True(result.AttributeModifiers.Contains(attributeModifier));
            Assert.True(result.AttributeModifiers.Contains(otherImprovementAttributeModifier));
        }

        [Test]
        public void CombineWith_Always_CombinesDefenderAuraTemplates()
        {
            var subject = ConstructSubject();

            var result = subject.CombineWith(otherDefenderImprovement);

            Assert.True(result.AttributeModifiers.Contains(attributeModifier));
            Assert.True(result.AttributeModifiers.Contains(otherImprovementAttributeModifier));
        }

        [Test]
        public void CombineWith_UnitImprovement_ContainsConditionalHexOccupationBonusOfOtherImprovement()
        {
            var subject = ConstructSubject();

            var result = subject.CombineWith(otherUnitImprovement) as IUnitImprovement;

            Assert.NotNull(result);
            Assert.True(result.ConditionalHexOccupationBonuses.Contains(otherUnitImprovementConditionalHexOccupationBonus));
        }

        [Test]
        public void CombineWith_UnitImprovement_ContainsConditionalStructureOccupationBonus()
        {
            var subject = ConstructSubject();

            var result = subject.CombineWith(otherUnitImprovement) as IUnitImprovement;

            Assert.NotNull(result);
            Assert.True(result.ConditionalStructureOccupationBonuses.Contains(otherUnitImprovementConditionalStructureOccupationBonus));
        }
    }
}