using System.Collections.Generic;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IBuildModeTargetingComponent
    {
        IReadOnlyList<IBuildModeTargetable> FindTargets(Coord position);
    }
}