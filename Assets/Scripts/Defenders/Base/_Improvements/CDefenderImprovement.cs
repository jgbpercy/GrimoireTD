using System.Collections.Generic;
using GrimoireTD.Abilities;
using GrimoireTD.Defenders.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Attributes;

namespace GrimoireTD.Defenders
{
    public class CDefenderImprovement : IDefenderImprovement
    {
        public ICollection<INamedAttributeModifier<DeAttrName>> AttributeModifiers { get; }

        public ICollection<IHexOccupationBonus> FlatHexOccupationBonuses { get; }

        public ICollection<IAbilityTemplate> Abilities { get; }

        public ICollection<IDefenderAuraTemplate> Auras { get; }

        public CDefenderImprovement(
            ICollection<INamedAttributeModifier<DeAttrName>> attributeModifiers,
            ICollection<IHexOccupationBonus> flatHexOccupationBonuses,
            ICollection<IAbilityTemplate> abilities,
            ICollection<IDefenderAuraTemplate> auras)
        {
            AttributeModifiers = attributeModifiers;
            FlatHexOccupationBonuses = flatHexOccupationBonuses;
            Abilities = abilities;
            Auras = auras;
        }
    }
}