using System.Collections.Generic;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IBuildModeTargetingComponent
    {
        int Range { get; }

        IReadOnlyList<IBuildModeTargetable> FindTargets(Coord position);
    }
}