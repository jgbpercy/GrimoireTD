using System;
using UnityEngine;

[Serializable]
public class HexOccupationBonus : OccupationBonus {

    [SerializeField]
    private BaseHexTypeEnum hexType;

    public HexType HexType
    {
        get
        {
            return MapGenerator.Instance.HexTypeFromName(hexType);
        }
    }

}
