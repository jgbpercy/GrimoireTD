namespace GrimoireTD.Abilities.DefendMode
{
    public interface IDefendModeTargetingComponentTemplate
    {
        IDefendModeTargetingArgsTemplate TargetingRule { get; }

        IDefendModeTargetingComponent GenerateTargetingComponent();
    }
}