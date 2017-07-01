using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class DefendingEntityImprovement {

    [SerializeField]
    private NamedAttributeModifier[] attributeModifiers;

    [SerializeField]
    private HexOccupationBonus[] flatHexOccupationBonuses;

    [SerializeField]
    private AbilityTemplate[] abilities;
    
    public NamedAttributeModifier[] AttributeModifiers
    {
        get
        {
            return attributeModifiers;
        }
    }

    public HexOccupationBonus[] FlatHexOccupationBonuses
    {
        get
        {
            return flatHexOccupationBonuses;
        }
    }

    public AbilityTemplate[] Abilities
    {
        get
        {
            return abilities;
        }
    }

    public DefendingEntityImprovement Combine(DefendingEntityImprovement otherImprovement)
    {
        NamedAttributeModifier[] combinedAttributeModifiers = new NamedAttributeModifier[AttributeModifiers.Length + otherImprovement.AttributeModifiers.Length];
        HexOccupationBonus[] combinedFlatHexOccupationBonuses = new HexOccupationBonus[FlatHexOccupationBonuses.Length + otherImprovement.FlatHexOccupationBonuses.Length];
        AbilityTemplate[] combinedAbilities = new AbilityTemplate[Abilities.Length + otherImprovement.Abilities.Length];

        Array.Copy(AttributeModifiers, combinedAttributeModifiers, AttributeModifiers.Length);
        Array.Copy(otherImprovement.AttributeModifiers, 0, combinedAttributeModifiers, AttributeModifiers.Length, otherImprovement.AttributeModifiers.Length);

        Array.Copy(FlatHexOccupationBonuses, combinedFlatHexOccupationBonuses, flatHexOccupationBonuses.Length);
        Array.Copy(otherImprovement.FlatHexOccupationBonuses, 0, combinedFlatHexOccupationBonuses, FlatHexOccupationBonuses.Length, otherImprovement.FlatHexOccupationBonuses.Length);

        Array.Copy(Abilities, combinedAbilities, Abilities.Length);
        Array.Copy(otherImprovement.Abilities, 0, combinedAbilities, Abilities.Length, otherImprovement.Abilities.Length);

        return new DefendingEntityImprovement()
        {
            attributeModifiers = combinedAttributeModifiers,
            flatHexOccupationBonuses = combinedFlatHexOccupationBonuses,
            abilities = combinedAbilities
        };
    }
}
