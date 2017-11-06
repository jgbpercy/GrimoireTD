namespace GrimoireTD.Defenders.DefenderEffects
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

        IDefenderImprovement Improvement { get; }
    }
}