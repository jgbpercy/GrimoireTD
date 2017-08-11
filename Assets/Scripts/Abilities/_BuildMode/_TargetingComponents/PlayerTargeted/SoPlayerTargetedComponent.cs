using System;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.BuildMode
{
    public class SoPlayerTargetedComponent : SoBuildModeTargetingComponent, IPlayerTargetedComponent
    {
        public virtual bool IsValidTarget(DefendingEntity sourceDefendingEntity, IBuildModeTargetable potentialTarget)
        {
            throw new NotImplementedException("Base PlayerTargetedComponent cannot evaluate valid targets and should not be used.");
        }
    }
}