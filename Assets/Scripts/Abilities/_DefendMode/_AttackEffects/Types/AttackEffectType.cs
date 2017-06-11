using UnityEngine;

public class AttackEffectType : ScriptableObject {

    [SerializeField]
    private string effectName;

    public virtual string EffectName()
    {
        return effectName;
    }
}

public enum MetaDamageType
{
    PhysicalDamage,
    ElementalDamage,
    PlanarDamage
}

public enum SpecificDamageType
{
    PiercingDamage,
    BluntDamage,
    PoisonDamage,
    AcidDamage,
    FireDamage,
    ColdDamage,
    LightningDamage,
    EarthDamage,
    DarkDamage,
    LightDamage,
    ChaosDamage,
    PureDamage
}

public enum SlowType
{
    FrostSlow,
    MaimSlow,
    TrapSlow,
    DazeSlow
}

public enum SingularPersistentType
{
    ArmorReduction
}

public enum SingularInstantType
{
    ArmorCorrosion
}