namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public interface IAttackEffectType
    {
        string ShortName { get; }

        string EffectName();
    }
}