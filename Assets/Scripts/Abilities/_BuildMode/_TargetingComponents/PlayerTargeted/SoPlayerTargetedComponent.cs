using System;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class SoPlayerTargetedComponent : SoBuildModeTargetingComponent, IPlayerTargetedComponent
    {
        public virtual bool IsValidTarget(IDefendingEntity sourceDefendingEntity, IBuildModeTargetable potentialTarget, IReadOnlyMapData mapData)
        {
            throw new NotImplementedException("Base PlayerTargetedComponent cannot evaluate valid targets and should not be used.");
        }
    }
}