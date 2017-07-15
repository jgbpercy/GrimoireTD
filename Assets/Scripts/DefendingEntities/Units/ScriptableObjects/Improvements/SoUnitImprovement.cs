using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitImprovement", menuName = "Structures and Units/Unit Improvement")]
public class SoUnitImprovement : SoDefendingEntityImprovement, IUnitImprovement {

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
