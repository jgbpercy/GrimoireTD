using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    public class CResistanceModifier : IResistanceModifier
    {
        public ISpecificDamageEffectType DamageType { get; }

        public float Magnitude { get; }

        public CResistanceModifier(float magnitude, ISpecificDamageEffectType damageType)
        {
            Magnitude = magnitude;
            DamageType = damageType;
        }

        public string AsPercentageString
        {
            get
            {
                return Magnitude * 100 + "%";
            }
        }
    }
}
