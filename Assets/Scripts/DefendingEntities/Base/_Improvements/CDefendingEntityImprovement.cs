using System.Collections.Generic;
using GrimoireTD.Abilities;
using GrimoireTD.DefendingEntities.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Attributes;

namespace GrimoireTD.DefendingEntities
{
    public class CDefendingEntityImprovement : IDefendingEntityImprovement
    {
        public IEnumerable<INamedAttributeModifier<DefendingEntityAttributeName>> AttributeModifiers { get; }

        public IEnumerable<IHexOccupationBonus> FlatHexOccupationBonuses { get; }

        public IEnumerable<IAbilityTemplate> Abilities { get; }

        public IEnumerable<IDefenderAuraTemplate> Auras { get; }

        public CDefendingEntityImprovement(
            ICollection<INamedAttributeModifier<DefendingEntityAttributeName>> attributeModifiers,
            ICollection<IHexOccupationBonus> flatHexOccupationBonuses,
            ICollection<IAbilityTemplate> abilities,
            ICollection<IDefenderAuraTemplate> auras)
        {
            var tempAttributeModifiers = new INamedAttributeModifier<DefendingEntityAttributeName>[attributeModifiers.Count];
            var tempFlatHexOccupationBonuses = new IHexOccupationBonus[flatHexOccupationBonuses.Count];
            var tempAbilities = new IAbilityTemplate[abilities.Count];
            var tempAuras = new IDefenderAuraTemplate[auras.Count];

            attributeModifiers.CopyTo(tempAttributeModifiers, 0);
            flatHexOccupationBonuses.CopyTo(tempFlatHexOccupationBonuses, 0);
            abilities.CopyTo(tempAbilities, 0);
            auras.CopyTo(tempAuras, 0);

            AttributeModifiers = tempAttributeModifiers;
            FlatHexOccupationBonuses = tempFlatHexOccupationBonuses;
            Abilities = tempAbilities;
            Auras = tempAuras;
        }

        public IDefendingEntityImprovement Combine(IDefendingEntityImprovement otherImprovement)
        {
            return Combine(this, otherImprovement);
        }

        public static IDefendingEntityImprovement Combine(IDefendingEntityImprovement firstImprovement, IDefendingEntityImprovement secondImprovement)
        {
            List<INamedAttributeModifier<DefendingEntityAttributeName>> combinedAttributeModifiers = new List<INamedAttributeModifier<DefendingEntityAttributeName>>();
            combinedAttributeModifiers.AddRange(firstImprovement.AttributeModifiers);
            combinedAttributeModifiers.AddRange(secondImprovement.AttributeModifiers);

            List<IHexOccupationBonus> combinedFlatHexOccupationBonuses = new List<IHexOccupationBonus>();
            combinedFlatHexOccupationBonuses.AddRange(firstImprovement.FlatHexOccupationBonuses);
            combinedFlatHexOccupationBonuses.AddRange(secondImprovement.FlatHexOccupationBonuses);

            List<IAbilityTemplate> combinedAbilities = new List<IAbilityTemplate>();
            combinedAbilities.AddRange(firstImprovement.Abilities);
            combinedAbilities.AddRange(secondImprovement.Abilities);

            List<IDefenderAuraTemplate> combinedAuras = new List<IDefenderAuraTemplate>();
            combinedAuras.AddRange(firstImprovement.Auras);
            combinedAuras.AddRange(secondImprovement.Auras);

            return new CDefendingEntityImprovement(combinedAttributeModifiers, combinedFlatHexOccupationBonuses, combinedAbilities, combinedAuras);
        }
    }
}