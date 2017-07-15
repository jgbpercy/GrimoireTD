using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingComponentFloatRange : DMTargetingComponent {

    [SerializeField]
    private float baseRange;

    public float BaseRange
    {
        get
        {
            return baseRange;
        }
    }

    public float GetActualRange(DefendingEntity attachedToDefendingEntity)
    {
        return baseRange * (1 + attachedToDefendingEntity.GetAttribute(AttributeName.rangeBonus));
    }
}
