using System.Collections.Generic;

public interface IDefendingEntityImprovement {

    IEnumerable<NamedAttributeModifier> AttributeModifiers { get; }

    IEnumerable<HexOccupationBonus> FlatHexOccupationBonuses { get; }

    IEnumerable<AbilityTemplate> Abilities { get; }

    IEnumerable<DefenderAuraTemplate> Auras { get; }

    IDefendingEntityImprovement Combine(IDefendingEntityImprovement otherImprovement);
}
