namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public interface IBlockModifierEffectType : IModifierEffectType
    {
        ISpecificDamageEffectType BlockTypeToModify { get; }
    }
}