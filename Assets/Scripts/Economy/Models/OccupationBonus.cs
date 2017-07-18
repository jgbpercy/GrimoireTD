using System;
using UnityEngine;

[Serializable]
public abstract class OccupationBonus {

    [SerializeField]
    private EconomyTransaction resourceGain;

    public EconomyTransaction ResourceGain
    {
        get
        {
            return resourceGain;
        }
    }
}
