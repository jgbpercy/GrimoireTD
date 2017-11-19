using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Defenders;
using GrimoireTD.Defenders.Units;
using GrimoireTD.Defenders.DefenderEffects;
using GrimoireTD.Abilities;
using GrimoireTD.Economy;
using GrimoireTD.Attributes;

namespace GrimoireTD.Tests.UnitImprovementTests
{
    public class UnitImprovementTests : DefenderImprovementTests.DefenderImprovementTests
    {
        //Other Deps Passed To Ctor or SetUp
        private IHexOccupationBonus conditionalHexOccupationBonus = Substitute.For<IHexOccupationBonus>();
        private IStructureOccupationBonus conditionalStructureOccupationBonus = Substitute.For<IStructureOccupationBonus>();

        protected override CDefenderImprovement ConstructSubject()
        {
            return ConstructUnitImprovementSubject();
        }

        private CUnitImprovement ConstructUnitImprovementSubject()
        {
            return new CUnitImprovement(
                new List<INamedAttributeModifier<DeAttrName>> { attributeModifier },
                new List<IHexOccupationBonus> { flatHexOccupationBonus },
                new List<IAbilityTemplate> { abilityTemplate },
                new List<IDefenderAuraTemplate> { auraTemplate },
                new List<IHexOccupationBonus> { conditionalHexOccupationBonus },
                new List<IStructureOccupationBonus> { conditionalStructureOccupationBonus}
            );
        }

        [Test]
        public void Ctor_Always_AddsConditionalHexOccupationBonuses()
        {
            var subject = ConstructUnitImprovementSubject();

            Assert.True(subject.ConditionalHexOccupationBonuses.Contains(conditionalHexOccupationBonus));
        }

        [Test]
        public void Ctor_Always_AddsConditionalStructureOccupationBonuses()
        {
            var subject = ConstructUnitImprovementSubject();

            Assert.True(subject.ConditionalStructureOccupationBonuses.Contains(conditionalStructureOccupationBonus));
        }

        [Test]
        public void CombineWith_DefenderImprovement_ContainsThisImprovementsConditionalHexOccupationBonus()
        {
            var subject = ConstructUnitImprovementSubject();

            var result = subject.CombineWith(otherDefenderImprovement) as IUnitImprovement;

            Assert.NotNull(result);
            Assert.True(result.ConditionalHexOccupationBonuses.Contains(conditionalHexOccupationBonus));
        }

        [Test]
        public void CombineWith_DefenderImprovement_ContainsThisImprovementsConditionalStructureOccupationBonus()
        {
            var subject = ConstructUnitImprovementSubject();

            var result = subject.CombineWith(otherDefenderImprovement) as IUnitImprovement;

            Assert.NotNull(result);
            Assert.True(result.ConditionalStructureOccupationBonuses.Contains(conditionalStructureOccupationBonus));
        }

        [Test]
        public void CombineWith_UnitImprovement_CombinesConditionalHexOccupationBonuses()
        {
            var subject = ConstructUnitImprovementSubject();

            var result = subject.CombineWith(otherUnitImprovement) as IUnitImprovement;

            Assert.NotNull(result);
            Assert.True(result.ConditionalHexOccupationBonuses.Contains(conditionalHexOccupationBonus));
            Assert.True(result.ConditionalHexOccupationBonuses.Contains(otherUnitImprovementConditionalHexOccupationBonus));
        }

        [Test]
        public void CombineWith_UnitImprovement_CombinesConditionalStructureOccupationBonuses()
        {
            var subject = ConstructUnitImprovementSubject();

            var result = subject.CombineWith(otherUnitImprovement) as IUnitImprovement;

            Assert.NotNull(result);
            Assert.True(result.ConditionalStructureOccupationBonuses.Contains(conditionalStructureOccupationBonus));
            Assert.True(result.ConditionalStructureOccupationBonuses.Contains(otherUnitImprovementConditionalStructureOccupationBonus));
        }
    }
}