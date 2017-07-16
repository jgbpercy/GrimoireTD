using System.Collections.Generic;

public interface IDefendModeTargetingComponent {

    List<IDefendModeTargetable> FindTargets(DefendingEntity attachedToDefendingEntity);
}
