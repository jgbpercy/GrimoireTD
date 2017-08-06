using System;
using UnityEngine;

[Serializable]
public abstract class OccupationBonus {

    [SerializeField]
    private SEconomyTransaction resourceGain;

    public IEconomyTransaction ResourceGain
    {
        get
        {
            return resourceGain;
        }
    }
}
