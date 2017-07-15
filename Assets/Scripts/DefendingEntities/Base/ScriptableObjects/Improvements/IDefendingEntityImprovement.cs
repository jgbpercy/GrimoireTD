public interface IDefendingEntityImprovement {

    NamedAttributeModifier[] AttributeModifiers { get; }

    HexOccupationBonus[] FlatHexOccupationBonuses { get; }

    AbilityTemplate[] Abilities { get; }

    DefenderAuraTemplate[] Auras { get; }

    IDefendingEntityImprovement Combine(IDefendingEntityImprovement otherImprovement);
}
