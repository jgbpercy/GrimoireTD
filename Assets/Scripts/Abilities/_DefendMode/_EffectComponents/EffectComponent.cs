using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectComponent : ScriptableObject {

    public virtual void ExecuteEffect(Vector3 position, List<ITargetable> targets)
    {
        return;
    }
}
