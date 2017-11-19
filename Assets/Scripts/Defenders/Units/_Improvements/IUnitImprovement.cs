using System.Collections.Generic;
using GrimoireTD.Economy;

namespace GrimoireTD.Defenders.Units
{
    public interface IUnitImprovement : IDefenderImprovement
    {
        ICollection<IHexOccupationBonus> ConditionalHexOccupationBonuses { get; }

        ICollection<IStructureOccupationBonus> ConditionalStructureOccupationBonuses { get; }
    }
}