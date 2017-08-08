using UnityEngine;

[CreateAssetMenu(
    fileName = "NewSlowEffectType",
    menuName = "Attack Effects/Instant/Armor Corrosion"
)]
public class ArmorCorrosionEffectType : InstantEffectType
{
    public override string EffectName()
    {
        if (base.EffectName() == "")
        {
            return "Armor Corrosion";
        }

        return base.EffectName() + " Armor Corrosion";
    }
}