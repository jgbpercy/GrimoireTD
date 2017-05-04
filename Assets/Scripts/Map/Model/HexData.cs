using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HexData {

    private HexType hexType;

    private Structure structureHere = null;

    private Unit unitHere = null;

    public HexType HexType
    {
        get
        {
            return hexType;
        }
    }

    public Structure StructureHere
    {
        get
        {
            return structureHere;
        }
    }

    public Unit UnitHere
    {
        get
        {
            return unitHere;
        }
    }

    public float pathingFScore;
    public float pathingGScore;
    public Coord pathingCameFrom;

    public HexData(HexType createWithType)
    {
        hexType = createWithType;
        pathingFScore = Mathf.Infinity;
        pathingGScore = Mathf.Infinity;
    }

    public bool IsPathable()
    {
        return hexType.IsPathableByCreeps;
    }

    public bool IsBuildable()
    {
        return hexType.IsBuildable;
    }

    public bool CanAddStructureHere()
    {
        return IsBuildable() && structureHere == null;
    }

    public void AddStructureHere(Structure structureAdded)
    {
        structureHere = structureAdded;
    }
    
    public bool CanMoveUnitHere()
    {
        return hexType.UnitCanOccupy && unitHere == null;
    }    

    public void MoveUnitHere(Unit unitMoved)
    {
        unitHere = unitMoved;
    }

    public void RemoveUnitHere()
    {
        unitHere = null;
    }
}
