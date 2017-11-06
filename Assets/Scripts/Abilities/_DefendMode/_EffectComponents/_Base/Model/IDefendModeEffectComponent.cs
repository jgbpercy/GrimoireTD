using System.Collections.Generic;
using GrimoireTD.Defenders;

namespace GrimoireTD.Abilities.DefendMode
{
    public interface IDefendModeEffectComponent
    {
        void ExecuteEffect(IDefender attachedToDefender, IReadOnlyList<IDefendModeTargetable> targets);
    }
}