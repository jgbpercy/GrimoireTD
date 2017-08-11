using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    public interface IResistances
    {
        int Armor { get; }

        float GetResistance(DamageEffectType damageEffectType, int currentMinusArmor);

        int GetBlock(DamageEffectType damageEffectType);
    }
}