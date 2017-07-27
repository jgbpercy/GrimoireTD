using System.Collections.Generic;

public interface IBuildModeTargetingComponent {

    int Range { get; }

    IReadOnlyList<IBuildModeTargetable> FindTargets(Coord position);
}
