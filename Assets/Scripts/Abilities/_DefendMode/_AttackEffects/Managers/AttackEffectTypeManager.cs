using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffectTypeManager : SingletonMonobehaviour<AttackEffectTypeManager> {

    [SerializeField]
    private AttackEffectType[] attackEffectTypes;

    private Dictionary<MetaDamageEffectType, List<SpecificDamageEffectType>> metaToSpecificDictionary = new Dictionary<MetaDamageEffectType, List<SpecificDamageEffectType>>();

    private Dictionary<SpecificDamageEffectType, WeakMetaDamageEffectType> specificToWeakDictionary = new Dictionary<SpecificDamageEffectType, WeakMetaDamageEffectType>();
    private Dictionary<SpecificDamageEffectType, StrongMetaDamageEffectType> specificToStrongDictionary = new Dictionary<SpecificDamageEffectType, StrongMetaDamageEffectType>();

    private List<SpecificDamageEffectType> specificDamageTypes = new List<SpecificDamageEffectType>();
    private List<MetaDamageEffectType> metaDamageTypes = new List<MetaDamageEffectType>();
    private List<BasicMetaDamageEffectType> basicMetaDamageTypes = new List<BasicMetaDamageEffectType>();
    private List<WeakMetaDamageEffectType> weakMetaDamageTypes = new List<WeakMetaDamageEffectType>();
    private List<StrongMetaDamageEffectType> strongMetaDamageTypes = new List<StrongMetaDamageEffectType>();

    private List<InstantEffectType> instantEffectTypes = new List<InstantEffectType>();

    private List<ArmorCorrosionEffectType> armorCorrosionTypes = new List<ArmorCorrosionEffectType>();

    private List<PersistentEffectType> persistentEffectTypes = new List<PersistentEffectType>();

    private List<ArmorReductionEffectType> armorReductionTypes = new List<ArmorReductionEffectType>();
    private List<SlowEffectType> slowTypes = new List<SlowEffectType>();

    public IReadOnlyList<SpecificDamageEffectType> SpecificDamageTypes
    {
        get
        {
            return specificDamageTypes;
        }
    }
    public IReadOnlyList<MetaDamageEffectType> MetaDamageTypes
    {
        get
        {
            return metaDamageTypes;
        }
    }
    public IReadOnlyList<BasicMetaDamageEffectType> BasicMetaDamageTypes
    {
        get
        {
            return basicMetaDamageTypes;
        }
    }
    public IReadOnlyList<WeakMetaDamageEffectType> WeakMetaDamageTypes
    {
        get
        {
            return weakMetaDamageTypes;
        }
    }
    public IReadOnlyList<StrongMetaDamageEffectType> StrongMetaDamageTypes
    {
        get
        {
            return strongMetaDamageTypes;
        }
    }

    public IReadOnlyList<InstantEffectType> InstantEffectTypes
    {
        get
        {
            return instantEffectTypes;
        }
    }

    public IReadOnlyList<ArmorCorrosionEffectType> ArmorCorrosionTypes
    {
        get
        {
            return armorCorrosionTypes;
        }
    }

    public IReadOnlyList<PersistentEffectType> PersistentEffectTypes
    {
        get
        {
            return persistentEffectTypes;
        }
    }

    public IReadOnlyList<ArmorReductionEffectType> ArmorReductionTypes
    {
        get
        {
            return armorReductionTypes;
        }
    }
    public IReadOnlyList<SlowEffectType> SlowTypes
    {
        get
        {
            return slowTypes;
        }
    }

    private void Awake()
    {
        foreach (AttackEffectType attackEffectType in attackEffectTypes)
        {
            DamageEffectType damageEffectType = attackEffectType as DamageEffectType;
            if (damageEffectType != null)
            {
                AddDamageEffectType(damageEffectType);
                continue;
            }

            InstantEffectType instantEffectType = attackEffectType as InstantEffectType;
            if (instantEffectType != null)
            {
                AddInstantEffectType(instantEffectType);
                continue;
            }

            PersistentEffectType persistentEffectType = attackEffectType as PersistentEffectType;
            if (persistentEffectType != null)
            {
                AddPersistentEffectType(persistentEffectType);
                continue;
            }
        }

        foreach (BasicMetaDamageEffectType basicMetaDamageType in basicMetaDamageTypes)
        {
            metaToSpecificDictionary.Add(basicMetaDamageType, new List<SpecificDamageEffectType>());
        }

        foreach (SpecificDamageEffectType specificDamageType in specificDamageTypes)
        {
            metaToSpecificDictionary[specificDamageType.BasicMetaDamageEffectType].Add(specificDamageType);
        }

        foreach (WeakMetaDamageEffectType weakMetaDamageType in weakMetaDamageTypes)
        {
            metaToSpecificDictionary.Add(weakMetaDamageType, metaToSpecificDictionary[weakMetaDamageType.BasicMetaDamageType]);

            foreach (SpecificDamageEffectType specificDamageType in metaToSpecificDictionary[weakMetaDamageType])
            {
                specificToWeakDictionary.Add(specificDamageType, weakMetaDamageType);
            }
        }

        foreach (StrongMetaDamageEffectType strongMetaDamageType in strongMetaDamageTypes)
        {
            metaToSpecificDictionary.Add(strongMetaDamageType, metaToSpecificDictionary[strongMetaDamageType.BasicMetaDamageType]);

            foreach (SpecificDamageEffectType specificDamageType in metaToSpecificDictionary[strongMetaDamageType])
            {
                specificToStrongDictionary.Add(specificDamageType, strongMetaDamageType);
            }
        }
    }

    private void AddDamageEffectType(DamageEffectType damageEffectType)
    {
        SpecificDamageEffectType specificDamageEffectType = damageEffectType as SpecificDamageEffectType;
        if (specificDamageEffectType != null)
        {
            specificDamageTypes.Add(specificDamageEffectType);
            return;
        }

        BasicMetaDamageEffectType basicMetaDamageEffectType = damageEffectType as BasicMetaDamageEffectType;
        if (basicMetaDamageEffectType != null)
        {
            basicMetaDamageTypes.Add(basicMetaDamageEffectType);
            return;
        }

        WeakMetaDamageEffectType weakMetaDamageEffectType = damageEffectType as WeakMetaDamageEffectType;
        if (weakMetaDamageEffectType != null)
        {
            weakMetaDamageTypes.Add(weakMetaDamageEffectType);
            return;
        }

        StrongMetaDamageEffectType strongMetaDamageEffectType = damageEffectType as StrongMetaDamageEffectType;
        if (strongMetaDamageEffectType != null)
        {
            strongMetaDamageTypes.Add(strongMetaDamageEffectType);
            return;
        }

        throw new Exception("Unhandled damageEffectType");
    }

    private void AddInstantEffectType(InstantEffectType instantEffectType)
    {
        instantEffectTypes.Add(instantEffectType);

        ArmorCorrosionEffectType armorCorrosionEffectType = instantEffectType as ArmorCorrosionEffectType;
        if (armorCorrosionEffectType != null)
        {
            armorCorrosionTypes.Add(armorCorrosionEffectType);
            return;
        }

        throw new Exception("Unhandled instantEffectType");
    }

    private void AddPersistentEffectType(PersistentEffectType persistentEffectType)
    {
        persistentEffectTypes.Add(persistentEffectType);

        SlowEffectType slowEffectType = persistentEffectType as SlowEffectType;
        if (slowEffectType != null)
        {
            slowTypes.Add(slowEffectType);
            return;
        }

        ArmorReductionEffectType armorReductionEffectType = persistentEffectType as ArmorReductionEffectType;
        if (armorReductionEffectType != null)
        {
            armorReductionTypes.Add(armorReductionEffectType);
            return;
        }

        throw new Exception("Unhandled persistentEffectType");
    }

    public IReadOnlyList<SpecificDamageEffectType> GetSpecificDamageTypes(MetaDamageEffectType metaDamageEffectType)
    {
        return metaToSpecificDictionary[metaDamageEffectType];
    }

    public BasicMetaDamageEffectType GetBasicMetaDamageType(SpecificDamageEffectType specificDamageType)
    {
        return specificDamageType.BasicMetaDamageEffectType;
    }

    public WeakMetaDamageEffectType GetWeakMetaDamageType(SpecificDamageEffectType specificDamageType)
    {
        return specificToWeakDictionary[specificDamageType];
    }

    public StrongMetaDamageEffectType GetStrongMetaDamageType(SpecificDamageEffectType specificDamageType)
    {
        return specificToStrongDictionary[specificDamageType];
    }
}
