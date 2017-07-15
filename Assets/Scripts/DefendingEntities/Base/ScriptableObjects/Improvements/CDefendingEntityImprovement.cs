using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDefendingEntityImprovement : IDefendingEntityImprovement {

    private NamedAttributeModifier[] attributeModifiers;

    private HexOccupationBonus[] flatHexOccupationBonuses;

    private AbilityTemplate[] abilities;

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

    public CDefendingEntityImprovement(
        ICollection<NamedAttributeModifier> attributeModifiers, 
        ICollection<HexOccupationBonus> flatHexOccupationBonuses, 
        ICollection<AbilityTemplate> abilities, 
        ICollection<DefenderAuraTemplate> auras)
    {
        this.attributeModifiers = new NamedAttributeModifier[attributeModifiers.Count];
        this.flatHexOccupationBonuses = new HexOccupationBonus[flatHexOccupationBonuses.Count];
        this.abilities = new AbilityTemplate[abilities.Count];
        this.auras = new DefenderAuraTemplate[auras.Count];

        attributeModifiers.CopyTo(this.attributeModifiers, 0);
        flatHexOccupationBonuses.CopyTo(this.flatHexOccupationBonuses, 0);
        abilities.CopyTo(this.abilities, 0);
        auras.CopyTo(this.auras, 0);
    }

    public IDefendingEntityImprovement Combine(IDefendingEntityImprovement otherImprovement)
    {
        return SoDefendingEntityImprovement.Combine(this, otherImprovement);
    }
}
