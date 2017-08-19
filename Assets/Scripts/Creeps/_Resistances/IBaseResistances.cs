using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    public interface IBaseResistances
    {
        IBlockModifier GetBlockModifier(ISpecificDamageEffectType specificDamageEffectType);

        IResistanceModifier GetResistanceModifier(ISpecificDamageEffectType specificDamageEffectType);
    }
}