using System.Collections.Generic;

namespace GrimoireTD.DefendingEntities.Structures
{
    public interface IStructureUpgrade
    {
        IEnumerable<IStructureEnhancement> OptionalEnhancements { get; }

        IDefendingEntityImprovement MainUpgradeBonus { get; }

        string NewStructureName { get; }

        string NewStructureDescription { get; }

        string BonusDescription { get; }
    }
}