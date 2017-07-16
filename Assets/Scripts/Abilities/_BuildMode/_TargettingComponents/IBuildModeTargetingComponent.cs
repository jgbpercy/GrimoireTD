using System.Collections.Generic;

public interface IBuildModeTargetingComponent {

    int Range { get; }

    List<IBuildModeTargetable> FindTargets(Coord position);
}
