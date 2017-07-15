using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitImprovement : IDefendingEntityImprovement {

    HexOccupationBonus[] ConditionalHexOccupationBonuses { get; }

    StructureOccupationBonus[] ConditionalStructureOccupationBonuses { get; }

}
