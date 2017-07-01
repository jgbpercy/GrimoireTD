using System;
using UnityEngine;

[Serializable]
public class UnitImprovement : DefendingEntityImprovement {

    [SerializeField]
    private HexOccupationBonus[] conditionalHexOccupationBonuses;

    [SerializeField]
    private StructureOccupationBonus[] conditionalStructureOccupationBonuses;

    public HexOccupationBonus[] ConditionalHexOccupationBonuses
    {
        get
        {
            return conditionalHexOccupationBonuses;
        }
    }

    public StructureOccupationBonus[] ConditionalStructureOccupationBonuses
    {
        get
        {
            return conditionalStructureOccupationBonuses;
        }
    }

}
