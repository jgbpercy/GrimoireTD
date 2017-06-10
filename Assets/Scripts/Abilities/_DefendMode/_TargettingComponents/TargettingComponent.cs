using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingComponent : ScriptableObject {

    public virtual List<ITargetable> FindTargets(Vector3 position)
    {
        return null;
    }
}
