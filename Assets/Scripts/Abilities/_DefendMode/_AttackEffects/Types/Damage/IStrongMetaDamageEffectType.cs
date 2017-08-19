namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public interface IStrongMetaDamageEffectType : IMetaDamageEffectType
    {
        IBasicMetaDamageEffectType BasicMetaDamageType { get; }
    }
}