using System.Collections.Generic;

namespace GrimoireTD.Defenders.Structures
{
    public interface IStructureUpgrade
    {
        IEnumerable<IStructureEnhancement> OptionalEnhancements { get; }

        IDefenderImprovement MainUpgradeBonus { get; }

        string NewStructureName { get; }

        string NewStructureDescription { get; }

        string BonusDescription { get; }
    }
}