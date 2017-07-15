using System;

public class PlayerTargetedComponent : BMTargetingComponent {

    public virtual bool IsValidTarget(DefendingEntity sourceDefendingEntity, IBuildModeTargetable potentialTarget)
    {
        throw new NotImplementedException("Base PlayerTargetedComponent cannot evaluate valid targets and should not be used.");
    }
}