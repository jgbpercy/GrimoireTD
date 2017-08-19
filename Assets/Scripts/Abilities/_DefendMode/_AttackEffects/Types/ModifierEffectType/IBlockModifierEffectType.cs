namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public interface IBlockModifierEffectType
    {
        ISpecificDamageEffectType BlockTypeToModify { get; }
    }
}