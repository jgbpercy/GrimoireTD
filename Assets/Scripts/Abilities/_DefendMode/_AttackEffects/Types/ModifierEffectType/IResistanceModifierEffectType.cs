namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public interface IResistanceModifierEffectType
    {
        ISpecificDamageEffectType ResistanceToModify { get; }
    }
}