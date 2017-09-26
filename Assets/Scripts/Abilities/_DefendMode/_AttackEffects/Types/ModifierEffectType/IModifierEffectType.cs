namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public interface IModifierEffectType : IAttackEffectType
    {
        bool Temporary { get; }
    }
}