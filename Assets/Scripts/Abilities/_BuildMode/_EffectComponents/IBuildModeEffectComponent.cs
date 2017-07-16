using System.Collections.Generic;

public interface IBuildModeEffectComponent {

    void ExecuteEffect(DefendingEntity executingEntity, List<IBuildModeTargetable> targets);
}
