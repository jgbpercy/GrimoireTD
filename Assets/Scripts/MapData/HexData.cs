using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HexData {


    public HexType HexType
    {
        get
        {
            return hexType;
        }
    }

    private HexType hexType;

    public Tower StructureHere
    {
        get
        {
            return structureHere;
        }
    }

    private Tower structureHere = null;

    public float pathingFScore;
    public float pathingGScore;
    public Coord pathingCameFrom;

    public HexData()
    {
        hexType = HexType.HEX_GRASS;
        pathingFScore = Mathf.Infinity;
        pathingGScore = Mathf.Infinity;

    }

    public HexData(HexType createWithType)
    {
        hexType = createWithType;
        pathingFScore = Mathf.Infinity;
        pathingGScore = Mathf.Infinity;
    }

    public bool isPathable()
    {
        return hexType == HexType.HEX_GRASS ? true : false;
    }

    public bool isBuildable()
    {
        return HexType == HexType.HEX_ROCK ? true : false;
    }

    public bool canAddStructureHere()
    {
        return isBuildable() && structureHere == null;
    }

    public bool tryAddStructureHere(Tower structureAdded)
    {
        if (canAddStructureHere())
        {
            structureHere = structureAdded;
            return true;
        }
        return false;
    }
    
}
