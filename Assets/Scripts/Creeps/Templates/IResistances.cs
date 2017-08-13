using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    public interface IResistances
    {
        float GetResistance(DamageEffectType damageEffectType, float currentArmor);

        int GetBlock(DamageEffectType damageEffectType);
    }
}