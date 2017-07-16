public interface ITargetingComponentFloatRange {

    float BaseRange { get; }

    float GetActualRange(DefendingEntity attachedToDefendingEntity);
}
