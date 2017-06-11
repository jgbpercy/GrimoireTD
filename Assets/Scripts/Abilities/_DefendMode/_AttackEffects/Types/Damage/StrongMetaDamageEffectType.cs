using UnityEngine;

[CreateAssetMenu(
    fileName = "NewStrongMetaDamageEffectType",
    menuName = "Attack Effects/Damage Types/Strong Meta Damage"
)]
public class StrongMetaDamageEffectType : MetaDamageEffectType {

    public override string EffectName()
    {
        return "Strong " + base.EffectName();
    }
}
