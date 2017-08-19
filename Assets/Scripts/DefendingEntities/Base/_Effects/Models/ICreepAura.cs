namespace GrimoireTD.DefendingEntities.DefenderEffects
{
    public interface ICreepAura : IDefenderEffect
    {
        ICreepAuraTemplate CreepAuraTemplate { get; }

        float Range { get; }
    }
}