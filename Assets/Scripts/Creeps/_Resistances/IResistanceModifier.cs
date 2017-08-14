using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    public interface IResistanceModifier
    {
        float Magnitude { get; }

        SpecificDamageEffectType DamageType { get; }

        string AsPercentageString { get; }
    }
}