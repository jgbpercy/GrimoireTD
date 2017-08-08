public interface IResistances
{
    int Armor { get; }

    float GetResistance(DamageEffectType damageEffectType, int currentMinusArmor);

    int GetBlock(DamageEffectType damageEffectType);
}
