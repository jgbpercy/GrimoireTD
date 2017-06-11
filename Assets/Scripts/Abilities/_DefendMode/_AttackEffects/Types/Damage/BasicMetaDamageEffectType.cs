using UnityEngine;

[CreateAssetMenu(
    fileName = "NewBasicMetaDamageEffectType",
    menuName = "Attack Effects/Damage Types/Basic Meta Damage"
)]
public class BasicMetaDamageEffectType : MetaDamageEffectType {

    public override string EffectName()
    {
        return "Basic " + base.EffectName();
    }
}
