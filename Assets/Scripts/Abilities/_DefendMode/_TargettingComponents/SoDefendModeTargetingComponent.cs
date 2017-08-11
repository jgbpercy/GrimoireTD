using System;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.DefendMode
{
    public class SoDefendModeTargetingComponent : ScriptableObject, IDefendModeTargetingComponent
    {
        public virtual IReadOnlyList<IDefendModeTargetable> FindTargets(DefendingEntity attachedToDefendingEntity)
        {
            throw new NotImplementedException("Base DMTargetingComponent cannot find targets and should not be used.");
        }
    }
}