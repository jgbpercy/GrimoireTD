using System;
using System.Collections.Generic;

namespace GrimoireTD.DefendingEntities.Structures
{
    public interface IStructure : IDefendingEntity
    {
        IStructureTemplate StructureTemplate { get; }

        string CurrentDescription { get; }

        IReadOnlyDictionary<IStructureUpgrade, bool> UpgradesBought { get; }

        IReadOnlyDictionary<IStructureEnhancement, bool> EnhancementsChosen { get; }

        event EventHandler<EAOnUpgraded> OnUpgraded;

        IStructureUpgrade CurrentUpgradeLevel();

        bool TryUpgrade(IStructureUpgrade upgrade, IStructureEnhancement chosenEnhancement);
    }
}