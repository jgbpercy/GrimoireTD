﻿using System;

namespace GrimoireTD.DefendingEntities.Structures
{
    public class EAOnUpgraded : EventArgs
    {
        public readonly IStructureUpgrade Upgrade;

        public readonly IStructureEnhancement Enhancement;

        public EAOnUpgraded(IStructureUpgrade upgrade, IStructureEnhancement enhancement)
        {
            Upgrade = upgrade;
            Enhancement = enhancement;
        }
    }
}