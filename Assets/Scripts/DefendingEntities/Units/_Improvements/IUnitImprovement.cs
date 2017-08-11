using System.Collections.Generic;
using GrimoireTD.Economy;

namespace GrimoireTD.DefendingEntities.Units
{
    public interface IUnitImprovement : IDefendingEntityImprovement
    {
        IEnumerable<HexOccupationBonus> ConditionalHexOccupationBonuses { get; }

        IEnumerable<StructureOccupationBonus> ConditionalStructureOccupationBonuses { get; }
    }
}