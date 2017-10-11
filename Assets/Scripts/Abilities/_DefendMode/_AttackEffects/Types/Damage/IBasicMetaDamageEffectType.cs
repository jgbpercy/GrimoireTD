namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public interface IBasicMetaDamageEffectType : IMetaDamageEffectType
    {
        IWeakMetaDamageEffectType WeakMetaDamageType { get; }

        IStrongMetaDamageEffectType StrongMetaDamageType { get; }
    }
}