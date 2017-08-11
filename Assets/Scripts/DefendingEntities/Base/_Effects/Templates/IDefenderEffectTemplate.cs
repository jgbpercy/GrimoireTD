namespace GrimoireTD.DefendingEntities.DefenderEffects
{
    public enum DefenderEffectAffectsType
    {
        UNITS,
        STRUCTURES,
        BOTH
    }

    public interface IDefenderEffectTemplate
    {
        DefenderEffectAffectsType Affects { get; }

        string NameInGame { get; }

        IDefendingEntityImprovement Improvement { get; }
    }
}