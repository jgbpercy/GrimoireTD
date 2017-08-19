namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public interface ISpecificDamageEffectType : IDamageEffectType
    {
        IBasicMetaDamageEffectType BasicMetaDamageEffectType { get; }
    }
}