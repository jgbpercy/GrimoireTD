using System.Collections.Generic;

public interface IDefendModeEffectComponent
{
    void ExecuteEffect(DefendingEntity attachedToDefendingEntity, List<IDefendModeTargetable> targets);
}
