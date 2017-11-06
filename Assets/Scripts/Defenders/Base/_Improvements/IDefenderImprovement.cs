using System.Collections.Generic;
using GrimoireTD.Abilities;
using GrimoireTD.Defenders.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Attributes;

namespace GrimoireTD.Defenders
{
    public interface IDefenderImprovement
    {
        IEnumerable<INamedAttributeModifier<DeAttrName>> AttributeModifiers { get; }

        IEnumerable<IHexOccupationBonus> FlatHexOccupationBonuses { get; }

        IEnumerable<IAbilityTemplate> Abilities { get; }

        IEnumerable<IDefenderAuraTemplate> Auras { get; }

        IDefenderImprovement Combine(IDefenderImprovement otherImprovement);
    }
}