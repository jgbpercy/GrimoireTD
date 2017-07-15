using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HexData {

    private HexType hexType;

    private Structure structureHere = null;

    private Unit unitHere = null;

    private CallbackList<DefenderAura> defenderAurasHere;

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

    public List<DefenderAura> DefenderAurasHere
    {
        get
        {
            return defenderAurasHere.List;
        }
    }

    public float pathingFScore;
    public float pathingGScore;
    public Coord pathingCameFrom;

    public HexData(HexType createWithType)
    {
        hexType = createWithType;
        ResetPathingData();

        defenderAurasHere = new CallbackList<DefenderAura>();
    }

    public void ResetPathingData()
    {
        pathingFScore = Mathf.Infinity;
        pathingGScore = Mathf.Infinity;
        pathingCameFrom = null;
    }

    //Public non-changing helpers
    public bool IsPathableByCreeps()
    {
        return hexType.TypeIsPathableByCreeps && IsEmpty();
    }

    public bool IsPathableByCreepsWithUnitRemoved()
    {
        return hexType.TypeIsPathableByCreeps && structureHere == null;
    }

    public bool IsPathableByCreepsWithTypePathable()
    {
        return IsEmpty();
    }

    public bool IsPathableByCreepsWithStructureRemoved()
    {
        return hexType.TypeIsPathableByCreeps && unitHere == null;
    }

    public bool CanPlaceStructureHere()
    {
        return hexType.IsBuildable && structureHere == null;
    }

    public bool CanPlaceUnitHere()
    {
        return hexType.UnitCanOccupy && unitHere == null;
    }

    public bool IsEmpty()
    {
        return structureHere == null && unitHere == null;
    }

    //Public change methods
    public void AddStructureHere(Structure structureAdded)
    {
        structureHere = structureAdded;
    }

    public void PlaceUnitHere(Unit unitPlaced)
    {
        unitHere = unitPlaced;
    }

    public void RemoveUnitHere()
    {
        unitHere = null;
    }
    
    public void AddDefenderAura(DefenderAura aura)
    {
        defenderAurasHere.Add(aura);

        aura.RegisterForOnClearAuraCallback(OnClearDefenderAura);
    }

    private void OnClearDefenderAura(DefenderAura aura)
    {
        defenderAurasHere.TryRemove(aura);

        aura.DeregisterForOnClearAuraCallback(OnClearDefenderAura);
    }

    //Callbacks
    public void RegisterForOnDefenderAuraAddedCallback(Action<DefenderAura> callback)
    {
        defenderAurasHere.RegisterForAdd(callback);
    }

    public void DeregisterForOnDefenderAuraAddedCallback(Action<DefenderAura> callback)
    {
        defenderAurasHere.DeregisterForAdd(callback);
    }

    public void RegisterForOnDefenderAuraRemovedCallback(Action<DefenderAura> callback)
    {
        defenderAurasHere.RegisterForRemove(callback);
    }

    public void DeregisterForOnDefenderAuraRemovedCallback(Action<DefenderAura> callback)
    {
        defenderAurasHere.DeregisterForRemove(callback);
    }
}
