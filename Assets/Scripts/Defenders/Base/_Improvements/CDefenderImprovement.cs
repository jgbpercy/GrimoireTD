using System.Collections.Generic;
using GrimoireTD.Abilities;
using GrimoireTD.Defenders.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Attributes;

namespace GrimoireTD.Defenders
{
    public class CDefenderImprovement : IDefenderImprovement
    {
        public IEnumerable<INamedAttributeModifier<DeAttrName>> AttributeModifiers { get; }

        public IEnumerable<IHexOccupationBonus> FlatHexOccupationBonuses { get; }

        public IEnumerable<IAbilityTemplate> Abilities { get; }

        public IEnumerable<IDefenderAuraTemplate> Auras { get; }

        public CDefenderImprovement(
            ICollection<INamedAttributeModifier<DeAttrName>> attributeModifiers,
            ICollection<IHexOccupationBonus> flatHexOccupationBonuses,
            ICollection<IAbilityTemplate> abilities,
            ICollection<IDefenderAuraTemplate> auras)
        {
            var tempAttributeModifiers = new INamedAttributeModifier<DeAttrName>[attributeModifiers.Count];
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

        public IDefenderImprovement Combine(IDefenderImprovement otherImprovement)
        {
            return Combine(this, otherImprovement);
        }

        public static IDefenderImprovement Combine(IDefenderImprovement firstImprovement, IDefenderImprovement secondImprovement)
        {
            List<INamedAttributeModifier<DeAttrName>> combinedAttributeModifiers = new List<INamedAttributeModifier<DeAttrName>>();
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

            return new CDefenderImprovement(combinedAttributeModifiers, combinedFlatHexOccupationBonuses, combinedAbilities, combinedAuras);
        }
    }
}