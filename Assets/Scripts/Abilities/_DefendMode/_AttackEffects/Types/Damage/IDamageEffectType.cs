namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public interface IDamageEffectType : IAttackEffectType
    {
        float EffectOfArmor { get; }
    }
}