using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingComponentFloatRange : TargetingComponent {

    [SerializeField]
    protected float range;

    public float Range
    {
        get
        {
            return range;
        }
    }
}
