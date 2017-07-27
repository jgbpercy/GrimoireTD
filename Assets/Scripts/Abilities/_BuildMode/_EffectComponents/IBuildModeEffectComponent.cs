using System.Collections.Generic;

public interface IBuildModeEffectComponent {

    void ExecuteEffect(DefendingEntity executingEntity, IReadOnlyList<IBuildModeTargetable> targets);
}
