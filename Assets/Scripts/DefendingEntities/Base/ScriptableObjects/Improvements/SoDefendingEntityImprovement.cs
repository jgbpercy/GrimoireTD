using UnityEngine;
using System;
using System.Collections;

[CreateAssetMenu(fileName = "NewDefendingEntityImprovement", menuName = "Structures and Units/Defending Entity Improvement")]
public class SoDefendingEntityImprovement : ScriptableObject, IDefendingEntityImprovement {

    [SerializeField]
    private NamedAttributeModifier[] attributeModifiers;

    [SerializeField]
    private HexOccupationBonus[] flatHexOccupationBonuses;

    [SerializeField]
    private AbilityTemplate[] abilities;

    [SerializeField]
    private DefenderAuraTemplate[] auras;
    
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

    public DefenderAuraTemplate[] Auras
    {
        get
        {
            return auras;
        }
    }

    public static IDefendingEntityImprovement Combine(IDefendingEntityImprovement firstImprovement, IDefendingEntityImprovement secondImprovement)
    {
        NamedAttributeModifier[] combinedAttributeModifiers = new NamedAttributeModifier[firstImprovement.AttributeModifiers.Length + secondImprovement.AttributeModifiers.Length];
        HexOccupationBonus[] combinedFlatHexOccupationBonuses = new HexOccupationBonus[firstImprovement.FlatHexOccupationBonuses.Length + secondImprovement.FlatHexOccupationBonuses.Length];
        AbilityTemplate[] combinedAbilities = new AbilityTemplate[firstImprovement.Abilities.Length + secondImprovement.Abilities.Length];
        DefenderAuraTemplate[] combinedAuras = new DefenderAuraTemplate[firstImprovement.Auras.Length + secondImprovement.Auras.Length];

        Array.Copy(firstImprovement.AttributeModifiers, combinedAttributeModifiers, firstImprovement.AttributeModifiers.Length);
        Array.Copy(secondImprovement.AttributeModifiers, 0, combinedAttributeModifiers, firstImprovement.AttributeModifiers.Length, secondImprovement.AttributeModifiers.Length);

        Array.Copy(firstImprovement.FlatHexOccupationBonuses, combinedFlatHexOccupationBonuses, firstImprovement.FlatHexOccupationBonuses.Length);
        Array.Copy(secondImprovement.FlatHexOccupationBonuses, 0, combinedFlatHexOccupationBonuses, firstImprovement.FlatHexOccupationBonuses.Length, secondImprovement.FlatHexOccupationBonuses.Length);

        Array.Copy(firstImprovement.Abilities, combinedAbilities, firstImprovement.Abilities.Length);
        Array.Copy(secondImprovement.Abilities, 0, combinedAbilities, firstImprovement.Abilities.Length, secondImprovement.Abilities.Length);

        Array.Copy(firstImprovement.Auras, combinedAuras, firstImprovement.Auras.Length);
        Array.Copy(secondImprovement.Auras, 0, combinedAuras, firstImprovement.Auras.Length, secondImprovement.Auras.Length);

        return new CDefendingEntityImprovement(combinedAttributeModifiers, combinedFlatHexOccupationBonuses, combinedAbilities, combinedAuras);
    }

    public IDefendingEntityImprovement Combine(IDefendingEntityImprovement otherImprovement)
    {
        return Combine(this, otherImprovement);
    }
}
