using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    public class CBlockModifier : IBlockModifier
    {
        public ISpecificDamageEffectType DamageType { get; }

        public int Magnitude { get; }

        public CBlockModifier(int magnitude, ISpecificDamageEffectType damageType)
        {
            Magnitude = magnitude;
            DamageType = damageType;
        }
    }
}