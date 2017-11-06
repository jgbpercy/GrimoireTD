using GrimoireTD.Defenders;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public interface IAttackEffect
    {
        IAttackEffectType AttackEffectType { get; }

        float BaseDuration { get; }

        float BaseMagnitude { get; }

        string EffectName { get; }

        float GetActualDuration(IDefender sourceDefender);

        float GetActualMagnitude(IDefender sourceDefender);
    }
}