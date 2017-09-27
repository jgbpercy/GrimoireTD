namespace GrimoireTD.Abilities.BuildMode
{
    public interface IPlayerTargetsDefendingEntityComponentTemplate : IPlayerTargetedComponentTemplate
    {
        IPlayerTargetsDefendingEntityArgsTemplate TargetingRule { get; }
    }
}