namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public class SoDamageEffectType : SoAttackEffectType, IDamageEffectType
    {
        public override string EffectName()
        {
            return base.EffectName() + " Damage";
        }

        public virtual float EffectOfArmor
        {
            get
            {
                return 0f;
            }
        }
    }
}