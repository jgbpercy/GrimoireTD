using System;
using UnityEngine;

[Serializable]
public class StartingUnit
{
    [SerializeField]
    private SoUnitTemplate unitTemplate;
    [SerializeField]
    private Coord startingPosition;

    public IUnitTemplate UnitTemplate
    {
        get
        {
            return unitTemplate;
        }
    }

    public Coord StartingPosition
    {
        get
        {
            return startingPosition;
        }
    }
}
