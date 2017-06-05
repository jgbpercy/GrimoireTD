using System;
using UnityEngine;

[Serializable]
public class HexOccupationBonus {

    [SerializeField]
    private BaseHexTypeEnum hexType;

    [SerializeField]
    private EconomyTransaction resourceGain;

    public HexType HexType
    {
        get
        {
            return MapGenerator.Instance.HexTypeFromName(hexType);
        }
    }

    public EconomyTransaction ResourceGain
    {
        get
        {
            return resourceGain;
        }
    }
}
