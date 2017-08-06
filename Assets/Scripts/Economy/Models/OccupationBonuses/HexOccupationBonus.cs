using System;
using UnityEngine;

[Serializable]
public class HexOccupationBonus : OccupationBonus {

    [SerializeField]
    private SoHexType hexType;

    public IHexType HexType
    {
        get
        {
            return hexType;
        }
    }

}
