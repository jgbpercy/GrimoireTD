using System.Collections.Generic;
using GrimoireTD.Economy;

namespace GrimoireTD.DefendingEntities.Units
{
    public interface IUnitImprovement : IDefendingEntityImprovement
    {
        IEnumerable<IHexOccupationBonus> ConditionalHexOccupationBonuses { get; }

        IEnumerable<IStructureOccupationBonus> ConditionalStructureOccupationBonuses { get; }
    }
}