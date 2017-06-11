using UnityEngine;

[CreateAssetMenu(
    fileName = "NewWeakMetaDamageEffectType", 
    menuName = "Attack Effects/Damage Types/Weak Meta Damage"
)]
public class WeakMetaDamageEffectType : MetaDamageEffectType {

    public override string EffectName()
    {
        return "Weak " + base.EffectName();
    }
}
