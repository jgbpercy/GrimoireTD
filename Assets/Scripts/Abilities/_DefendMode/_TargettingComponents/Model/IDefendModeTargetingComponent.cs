using System.Collections.Generic;
using GrimoireTD.Defenders;
using GrimoireTD.Creeps;

namespace GrimoireTD.Abilities.DefendMode
{
    public interface IDefendModeTargetingComponent
    {
        IReadOnlyList<IDefendModeTargetable> FindTargets(
            IDefender attachedToDefender
        );
    }
}