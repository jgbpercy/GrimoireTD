namespace GrimoireTD.Abilities.BuildMode
{
    public interface IPlayerTargetsDefenderComponentTemplate : IPlayerTargetedComponentTemplate
    {
        IPlayerTargetsDefenderArgsTemplate TargetingRule { get; }
    }
}