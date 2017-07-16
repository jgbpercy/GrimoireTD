public interface IPlayerTargetedComponent {

    bool IsValidTarget(DefendingEntity sourceDefendingEntity, IBuildModeTargetable potentialTarget);
}
