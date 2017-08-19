namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public interface IBasicMetaDamageEffectType : IMetaDamageEffectType
    {
        float EffectOfArmor { get; }

        IWeakMetaDamageEffectType WeakMetaDamageType { get; }

        IStrongMetaDamageEffectType StrongMetaDamageType { get; }
    }
}