namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public interface IResistanceModifierEffectType : IModifierEffectType
    {
        ISpecificDamageEffectType ResistanceToModify { get; }
    }
}