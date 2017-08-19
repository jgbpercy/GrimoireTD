using System;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.DefendMode
{
    public class SoDefendModeEffectComponent : ScriptableObject, IDefendModeEffectComponent
    {
        public virtual void ExecuteEffect(IDefendingEntity attachedToDefendingEntity, IReadOnlyList<IDefendModeTargetable> targets)
        {
            throw new NotImplementedException("Base DMEffectComponent cannot execute effect and should not be used");
        }
    }
}