namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public class DamageEffectType : AttackEffectType
    {
        public override string EffectName()
        {
            return base.EffectName() + " Damage";
        }
    }
}