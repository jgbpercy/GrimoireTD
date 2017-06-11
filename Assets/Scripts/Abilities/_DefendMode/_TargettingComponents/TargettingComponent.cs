using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingComponent : ScriptableObject {

    public virtual List<ITargetable> FindTargets(DefendingEntity attachedToDefendingEntity)
    {
        return null;
    }
}
