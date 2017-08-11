using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    [CreateAssetMenu(
        fileName = "NewSlowEffectType",
        menuName = "Attack Effects/Persistent/Slow"
    )]
    public class SlowEffectType : PersistentEffectType
    {
        public override string EffectName()
        {
            return "Slow (" + base.EffectName() + ")";
        }
    }
}