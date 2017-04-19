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

    public Structure StructureHere
    {
        get
        {
            return structureHere;
        }
    }

    private Structure structureHere = null;

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

    public bool IsPathable()
    {
        return hexType == HexType.HEX_GRASS ? true : false;
    }

    public bool IsBuildable()
    {
        return HexType == HexType.HEX_ROCK ? true : false;
    }

    public bool CanAddStructureHere()
    {
        return IsBuildable() && structureHere == null;
    }

    public void AddStructureHere(Structure structureAdded)
    {
        structureHere = structureAdded;
    }
    
}
