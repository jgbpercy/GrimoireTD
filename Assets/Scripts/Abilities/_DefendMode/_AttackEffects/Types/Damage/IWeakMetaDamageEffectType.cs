namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public interface IWeakMetaDamageEffectType : IMetaDamageEffectType
    {
        IBasicMetaDamageEffectType BasicMetaDamageType { get; }
    }
}