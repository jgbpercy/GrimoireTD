using System.Collections.Generic;
using GrimoireTD.Abilities;
using GrimoireTD.Defenders.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Attributes;

namespace GrimoireTD.Defenders
{
    public interface IDefenderImprovement
    {
        ICollection<INamedAttributeModifier<DeAttrName>> AttributeModifiers { get; }

        ICollection<IHexOccupationBonus> FlatHexOccupationBonuses { get; }

        ICollection<IAbilityTemplate> Abilities { get; }

        ICollection<IDefenderAuraTemplate> Auras { get; }
    }
}