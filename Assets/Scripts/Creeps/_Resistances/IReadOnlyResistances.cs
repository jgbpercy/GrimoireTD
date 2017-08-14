using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    public interface IReadOnlyResistances
    {
        float GetResistance(DamageEffectType damageEffectType, float currentArmor);

        int GetBlock(DamageEffectType damageEffectType);
    }
}