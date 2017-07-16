using System;
using System.Collections.Generic;
using UnityEngine;

public class SoBuildModeTargetingComponent : ScriptableObject, IBuildModeTargetingComponent {

    [SerializeField]
    protected int range;

    public int Range
    {
        get
        {
            return range;
        }
    }

    public virtual List<IBuildModeTargetable> FindTargets(Coord position)
    {
        throw new NotImplementedException("Base BMTargetingComponent cannot find targets and should not be used.");
    }
}
