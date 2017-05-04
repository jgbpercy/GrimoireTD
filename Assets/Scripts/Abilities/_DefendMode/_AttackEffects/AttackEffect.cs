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
        Dictionary<AttackEffectType, string> names = new Dictionary<AttackEffectType, string>();

        names.Add(AttackEffectType.PiercingDamage, "Piercing Damage");
        names.Add(AttackEffectType.BluntDamage, "Blunt Damage");
        names.Add(AttackEffectType.PoisonDamage, "Poison Damage");
        names.Add(AttackEffectType.AcidDamage, "Acid Damage");
        names.Add(AttackEffectType.FireDamage, "Fire Damage");
        names.Add(AttackEffectType.ColdDamage, "Cold Damage");
        names.Add(AttackEffectType.LightningDamage, "Lightning Damage");
        names.Add(AttackEffectType.EarthDamage, "Earth Damage");
        names.Add(AttackEffectType.FrostSlow, "Slow (Frost)");
        names.Add(AttackEffectType.MaimSlow, "Slow (Maim)");
        names.Add(AttackEffectType.TrapSlow, "Slow (Trap)");
        names.Add(AttackEffectType.DazeSlow, "Slow (Daze)");
        names.Add(AttackEffectType.ArmorReduction, "Armor Reduction");
        names.Add(AttackEffectType.ArmorCorrosion, "Armor Corrosion");

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
