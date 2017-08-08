using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewStrongMetaDamageEffectType",
    menuName = "Attack Effects/Damage Types/Strong Meta Damage"
)]
public class StrongMetaDamageEffectType : MetaDamageEffectType
{
    [SerializeField]
    private BasicMetaDamageEffectType basicMetaDamageType;

    public BasicMetaDamageEffectType BasicMetaDamageType
    {
        get
        {
            return basicMetaDamageType;
        }
    }

    public override string EffectName()
    {
        return "Strong " + base.EffectName();
    }
}
