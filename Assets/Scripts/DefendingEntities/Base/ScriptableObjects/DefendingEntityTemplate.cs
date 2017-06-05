using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendingEntityTemplate : ScriptableObject {

    [SerializeField]
    protected AbilityTemplate[] baseAbilities;

    [SerializeField]
    protected GameObject prefab;

    [SerializeField]
    protected HexOccupationBonus[] baseFlatHexOccupationBonuses;

    public AbilityTemplate[] BaseAbilities
    {
        get
        {
            return baseAbilities;
        }
    }

    public GameObject Prefab
    {
        get
        {
            return prefab;
        }
    }

    public HexOccupationBonus[] BaseFlatHexOccupationBonuses
    {
        get
        {
            return baseFlatHexOccupationBonuses;
        }
    }

}
