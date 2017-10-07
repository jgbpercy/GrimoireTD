using System.Collections.Generic;
using GrimoireTD.Abilities;
using GrimoireTD.DefendingEntities.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Attributes;

namespace GrimoireTD.DefendingEntities
{
    public interface IDefendingEntityImprovement
    {
        IEnumerable<INamedAttributeModifier<DEAttrName>> AttributeModifiers { get; }

        IEnumerable<IHexOccupationBonus> FlatHexOccupationBonuses { get; }

        IEnumerable<IAbilityTemplate> Abilities { get; }

        IEnumerable<IDefenderAuraTemplate> Auras { get; }

        IDefendingEntityImprovement Combine(IDefendingEntityImprovement otherImprovement);
    }
}