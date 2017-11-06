using System.Collections.Generic;
using GrimoireTD.Economy;

namespace GrimoireTD.Defenders.Units
{
    public interface IUnitImprovement : IDefenderImprovement
    {
        IEnumerable<IHexOccupationBonus> ConditionalHexOccupationBonuses { get; }

        IEnumerable<IStructureOccupationBonus> ConditionalStructureOccupationBonuses { get; }
    }
}