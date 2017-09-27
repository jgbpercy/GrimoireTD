namespace GrimoireTD.Abilities.BuildMode
{
    public interface IPlayerTargetedComponentTemplate : IBuildModeTargetingComponentTemplate
    {
        IBuildModeAutoTargetedArgsTemplate AoeRule { get; }
    }
}