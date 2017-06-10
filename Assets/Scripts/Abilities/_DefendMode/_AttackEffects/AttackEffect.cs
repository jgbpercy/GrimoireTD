using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AttackEffectType
{
    PiercingDamage,
    BluntDamage,
    PoisonDamage,
    AcidDamage,
    FireDamage,
    ColdDamage,
    LightningDamage,
    EarthDamage,
    FrostSlow,
    MaimSlow,
    TrapSlow,
    DazeSlow,
    ArmorReduction,
    ArmorCorrosion
}

//TODO: make this less stupid
public static class AttackEffectNames
{
    private static Dictionary<AttackEffectType, string> names = null;

    public static string NameOf(AttackEffectType attackEffectType)
    {
        if ( names == null)
        {
            names = CreateAttackEffectNames();
        }

        return names[attackEffectType];
    }

    private static Dictionary<AttackEffectType, string> CreateAttackEffectNames()
    {
        Dictionary<AttackEffectType, string> names = new Dictionary<AttackEffectType, string>
        {
            { AttackEffectType.PiercingDamage, "Piercing Damage" },
            { AttackEffectType.BluntDamage, "Blunt Damage" },
            { AttackEffectType.PoisonDamage, "Poison Damage" },
            { AttackEffectType.AcidDamage, "Acid Damage" },
            { AttackEffectType.FireDamage, "Fire Damage" },
            { AttackEffectType.ColdDamage, "Cold Damage" },
            { AttackEffectType.LightningDamage, "Lightning Damage" },
            { AttackEffectType.EarthDamage, "Earth Damage" },
            { AttackEffectType.FrostSlow, "Slow (Frost)" },
            { AttackEffectType.MaimSlow, "Slow (Maim)" },
            { AttackEffectType.TrapSlow, "Slow (Trap)" },
            { AttackEffectType.DazeSlow, "Slow (Daze)" },
            { AttackEffectType.ArmorReduction, "Armor Reduction" },
            { AttackEffectType.ArmorCorrosion, "Armor Corrosion" }
        };
        return names;
    }
}

[Serializable]
public class AttackEffect {

    [SerializeField]
    private AttackEffectType attackEffectType;

    private string effectName = "";

    [SerializeField]
    private int magnitude;

    [SerializeField]
    private float duration;

    public AttackEffectType AttackEffectType
    {
        get
        {
            return attackEffectType;
        }
    }

    public string EffectName
    {
        get
        {
            if ( effectName == "" )
            {
                effectName = AttackEffectNames.NameOf(attackEffectType);
            }
            return effectName;
        }
    }

    public int Magnitude
    {
        get
        {
            return magnitude;
        }
    }

    public float Duration
    {
        get
        {
            return duration;
        }
    }

}
