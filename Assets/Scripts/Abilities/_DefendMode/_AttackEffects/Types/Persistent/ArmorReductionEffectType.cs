using UnityEngine;

[CreateAssetMenu(
    fileName = "NewSlowEffectType",
    menuName = "Attack Effects/Persistent/Armor Reduction"
)]
public class ArmorReductionEffectType : PersistentEffectType
{
    public override string EffectName()
    {
        if (base.EffectName() == "")
        {
            return "Armor Reduction";
        }

        return base.EffectName() + " Armor Reduction";
    }
}