namespace GrimoireTD.Abilities.BuildMode
{
    public interface IPlayerTargetsHexComponentTemplate : IPlayerTargetedComponentTemplate
    {
        IPlayerTargetsHexArgsTemplate TargetingRule { get; }
    }
}