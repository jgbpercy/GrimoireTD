using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectComponent : ScriptableObject {

    public virtual void ExecuteEffect(DefendingEntity attachedToDefendingEntity, List<ITargetable> targets)
    {
        return;
    }
}
