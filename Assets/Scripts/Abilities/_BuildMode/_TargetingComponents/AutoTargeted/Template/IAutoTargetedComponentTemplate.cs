namespace GrimoireTD.Abilities.BuildMode
{
    public interface IAutoTargetedComponentTemplate : IBuildModeTargetingComponentTemplate
    {
        IBuildModeAutoTargetedArgsTemplate TargetingRule { get; }
    }
}