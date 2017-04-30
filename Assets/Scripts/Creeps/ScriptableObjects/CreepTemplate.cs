using UnityEngine;
using System;

[Serializable]
public class Resistances
{
    [SerializeField]
    private int armor;

    [SerializeField]
    private float basePhysicalResistance;

    [SerializeField]
    private float piercingResistance;
    [SerializeField]
    private float bluntResistance;
    [SerializeField]
    private float poisonReistance;
    [SerializeField]
    private float acidResistance;

    [SerializeField]
    private float baseElementalResistance;

    [SerializeField]
    private float fireResistance;
    [SerializeField]
    private float coldResistance;
    [SerializeField]
    private float lightningResistance;
    [SerializeField]
    private float earthResistance;

    [SerializeField]
    private int damageBlock;

    [SerializeField]
    private int physicalBlock;

    [SerializeField]
    private int piercingBlock;
    [SerializeField]
    private int bluntBlock;
    [SerializeField]
    private int poisonBlock;
    [SerializeField]
    private int acidBlock;

    [SerializeField]
    private int elementalBlock;

    [SerializeField]
    private int fireBlock;
    [SerializeField]
    private int coldBlock;
    [SerializeField]
    private int lightningBlock;
    [SerializeField]
    private int earthBlock;

    public int Armor
    {
        get
        {
            return armor;
        }
    }

    public float BasePhysicalResistance
    {
        get
        {
            return basePhysicalResistance;
        }
    }

    public float PiercingResistance
    {
        get
        {
            return piercingResistance;
        }
    }

    public float BluntResistance
    {
        get
        {
            return bluntResistance;
        }
    }

    public float PoisonReistance
    {
        get
        {
            return poisonReistance;
        }
    }

    public float AcidResistance
    {
        get
        {
            return acidResistance;
        }
    }

    public float BaseElementalResistance
    {
        get
        {
            return baseElementalResistance;

        }
    }

    public float FireResistance
    {
        get
        {
            return fireResistance;
        }
    }

    public float ColdResistance
    {
        get
        {
            return coldResistance;
        }
    }

    public float LightningResistance
    {
        get
        {
            return lightningResistance;
        }
    }

    public float EarthResistance
    {
        get
        {
            return earthResistance;
        }
    }

    public int DamageBlock
    {
        get
        {
            return damageBlock;
        }
    }

    public int PhysicalBlock
    {
        get
        {
            return physicalBlock;
        }
    }

    public int PiercingBlock
    {
        get
        {
            return piercingBlock;
        }
    }

    public int BluntBlock
    {
        get
        {
            return bluntBlock;
        }
    }

    public int PoisonBlock
    {
        get
        {
            return poisonBlock;
        }
    }

    public int AcidBlock
    {
        get
        {
            return acidBlock;
        }
    }

    public int ElementalBlock
    {
        get
        {
            return elementalBlock;
        }
    }

    public int FireBlock
    {
        get
        {
            return fireBlock;
        }
    }

    public int ColdBlock
    {
        get
        {
            return coldBlock;
        }
    }

    public int LightningBlock
    {
        get
        {
            return lightningBlock;
        }
    }

    public int EarthBlock
    {
        get
        {
            return earthBlock;
        }
    }

}

[CreateAssetMenu(fileName = "NewCreep", menuName = "Creeps/Creep")]
public class CreepTemplate : ScriptableObject {

    [SerializeField]
    private string nameInGame;

    [SerializeField]
    private float baseSpeed;

    [SerializeField]
    private int maxHitpoints;

    [SerializeField]
    private GameObject creepPrefab;

    [SerializeField]
    private EconomyTransaction bounty;

    [SerializeField]
    private Resistances resistances;

    public string NameInGame
    {
        get
        {
            return nameInGame;
        }
    }

    public float BaseSpeed
    {
        get
        {
            return baseSpeed;
        }
    }

    public int MaxHitpoints
    {
        get
        {
            return maxHitpoints;
        }
    }

    public GameObject CreepPrefab
    {
        get
        {
            return creepPrefab;
        }
    }

    public EconomyTransaction Bounty
    {
        get
        {
            return bounty;
        }
    }

    public Resistances Resistances
    {
        get
        {
            return resistances;
        }
    }

    public virtual Creep GenerateCreep(Vector3 spawnPosition)
    {
        return new Creep(this, spawnPosition);
    }

}
