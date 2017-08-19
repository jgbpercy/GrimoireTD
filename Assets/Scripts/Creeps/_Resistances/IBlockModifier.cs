using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    public interface IBlockModifier
    {
        int Magnitude { get; }

        ISpecificDamageEffectType DamageType { get; }
    }
}