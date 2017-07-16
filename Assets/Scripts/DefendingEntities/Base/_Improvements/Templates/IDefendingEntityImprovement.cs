using System.Collections.Generic;

public interface IDefendingEntityImprovement {

    IEnumerable<NamedAttributeModifier> AttributeModifiers { get; }

    IEnumerable<HexOccupationBonus> FlatHexOccupationBonuses { get; }

    IEnumerable<IAbilityTemplate> Abilities { get; }

    IEnumerable<IDefenderAuraTemplate> Auras { get; }

    IDefendingEntityImprovement Combine(IDefendingEntityImprovement otherImprovement);
}
