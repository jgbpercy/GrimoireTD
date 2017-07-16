using System.Collections.Generic;

public interface IUnitImprovement : IDefendingEntityImprovement {

    IEnumerable<HexOccupationBonus> ConditionalHexOccupationBonuses { get; }

    IEnumerable<StructureOccupationBonus> ConditionalStructureOccupationBonuses { get; }

}
