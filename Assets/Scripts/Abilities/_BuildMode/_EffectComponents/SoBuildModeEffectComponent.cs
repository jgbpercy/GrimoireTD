using System;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.BuildMode
{
    public class SoBuildModeEffectComponent : ScriptableObject, IBuildModeEffectComponent
    {
        public virtual void ExecuteEffect(DefendingEntity executingEntity, IReadOnlyList<IBuildModeTargetable> targets)
        {
            throw new NotImplementedException("Base BMEffectComponent cannot executre effect and should not be used.");
        }
    }
}
