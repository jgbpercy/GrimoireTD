using System;
using System.Collections.Generic;
using UnityEngine;

public class SoDefendModeEffectComponent : ScriptableObject, IDefendModeEffectComponent {

    public virtual void ExecuteEffect(DefendingEntity attachedToDefendingEntity, List<IDefendModeTargetable> targets)
    {
        throw new NotImplementedException("Base DMEffectComponent cannot execute effect and should not be used");
    }
}
