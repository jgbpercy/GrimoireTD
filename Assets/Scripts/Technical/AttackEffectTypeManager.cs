using System.Collections.Generic;
using UnityEngine;

public class AttackEffectTypeManager : SingletonMonobehaviour<AttackEffectTypeManager> {

    [SerializeField]
    private AttackEffectType[] attackEffectTypes;

    //private Dictionary<SpecificDamageType, AttackEffectType> specificDamageTypeDict;
    //private Dictionary<MetaDamageType, AttackEffectType> metaDamageTypeDict;
    private Dictionary<SlowType, AttackEffectType> slowTypeDict;
    private Dictionary<SingularPersistentType, AttackEffectType> singularPersistentDict;

    private void Start()
    {
        //specificDamageTypeDict = new Dictionary<SpecificDamageType, AttackEffectType>();
        //metaDamageTypeDict = new Dictionary<MetaDamageType, AttackEffectType>();
        slowTypeDict = new Dictionary<SlowType, AttackEffectType>();
        singularPersistentDict = new Dictionary<SingularPersistentType, AttackEffectType>();

        foreach (AttackEffectType attackEffectType in attackEffectTypes)
        {
            SlowEffectType slowEffectType = attackEffectType as SlowEffectType;
            if ( slowEffectType != null )
            {
                slowTypeDict.Add(slowEffectType.SlowType, slowEffectType);
                continue;
            }

            SingularPersistentEffectType singularPersistentEffectType = attackEffectType as SingularPersistentEffectType;
            if ( singularPersistentEffectType != null )
            {
                singularPersistentDict.Add(singularPersistentEffectType.SingularPersistentType, singularPersistentEffectType);
            }
        }
    }

    //public static AttackEffectType GetAttackEffectType(SpecificDamageType specificDamageType)
    //{

    //}

    //public static AttackEffectType GetAttackEffectType(MetaDamageType metaDamageType)
    //{

    //}

    public static AttackEffectType GetAttackEffectType(SlowType slowType)
    {
        return Instance.slowTypeDict[slowType];
    }

    public static AttackEffectType GetAttackEffectType(SingularPersistentType singularPersistentType)
    {
        return Instance.singularPersistentDict[singularPersistentType];
    }
}
