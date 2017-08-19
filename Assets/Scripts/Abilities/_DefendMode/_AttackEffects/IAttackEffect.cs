using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public interface IAttackEffect
    {
        IAttackEffectType AttackEffectType { get; }

        float BaseDuration { get; }

        float BaseMagnitude { get; }

        string EffectName { get; }

        float GetActualDuration(IDefendingEntity sourceDefendingEntity);

        float GetActualMagnitude(IDefendingEntity sourceDefendingEntity);
    }
}