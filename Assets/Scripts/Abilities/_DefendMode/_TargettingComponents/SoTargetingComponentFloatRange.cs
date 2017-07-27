using UnityEngine;

public class SoTargetingComponentFloatRange : SoDefendModeTargetingComponent, ITargetingComponentFloatRange {

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
