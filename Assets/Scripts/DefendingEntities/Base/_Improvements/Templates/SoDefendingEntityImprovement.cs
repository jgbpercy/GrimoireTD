using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDefendingEntityImprovement", menuName = "Structures and Units/Defending Entity Improvement")]
public class SoDefendingEntityImprovement : ScriptableObject, IDefendingEntityImprovement {

    [SerializeField]
    private NamedAttributeModifier[] attributeModifiers;

    [SerializeField]
    private HexOccupationBonus[] flatHexOccupationBonuses;

    [SerializeField]
    private SoAbilityTemplate[] abilities;

    [SerializeField]
    private SoDefenderAuraTemplate[] auras;
    
    public IEnumerable<NamedAttributeModifier> AttributeModifiers
    {
        get
        {
            return attributeModifiers;
        }
    }

    public IEnumerable<HexOccupationBonus> FlatHexOccupationBonuses
    {
        get
        {
            return flatHexOccupationBonuses;
        }
    }

    public IEnumerable<IAbilityTemplate> Abilities
    {
        get
        {
            return abilities;
        }
    }

    public IEnumerable<IDefenderAuraTemplate> Auras
    {
        get
        {
            return auras;
        }
    }

    public static IDefendingEntityImprovement Combine(IDefendingEntityImprovement firstImprovement, IDefendingEntityImprovement secondImprovement)
    {
        List<NamedAttributeModifier> combinedAttributeModifiers = new List<NamedAttributeModifier>();
        combinedAttributeModifiers.AddRange(firstImprovement.AttributeModifiers);
        combinedAttributeModifiers.AddRange(secondImprovement.AttributeModifiers);

        List<HexOccupationBonus> combinedFlatHexOccupationBonuses = new List<HexOccupationBonus>();
        combinedFlatHexOccupationBonuses.AddRange(firstImprovement.FlatHexOccupationBonuses);
        combinedFlatHexOccupationBonuses.AddRange(secondImprovement.FlatHexOccupationBonuses);

        List<IAbilityTemplate> combinedAbilities = new List<IAbilityTemplate>();
        combinedAbilities.AddRange(firstImprovement.Abilities);
        combinedAbilities.AddRange(secondImprovement.Abilities);

        List<IDefenderAuraTemplate> combinedAuras = new List<IDefenderAuraTemplate>();
        combinedAuras.AddRange(firstImprovement.Auras);
        combinedAuras.AddRange(secondImprovement.Auras);

        return new CDefendingEntityImprovement(combinedAttributeModifiers, combinedFlatHexOccupationBonuses, combinedAbilities, combinedAuras);
    }

    public IDefendingEntityImprovement Combine(IDefendingEntityImprovement otherImprovement)
    {
        return Combine(this, otherImprovement);
    }
}
