using System.Collections.Generic;

public interface IDefendModeTargetingComponent {

    IReadOnlyList<IDefendModeTargetable> FindTargets(DefendingEntity attachedToDefendingEntity);
}
