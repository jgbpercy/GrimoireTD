﻿using System;
using System.Collections.Generic;
using GrimoireTD.DefendingEntities.DefenderEffects;
using GrimoireTD.DefendingEntities.Structures;
using GrimoireTD.DefendingEntities.Units;

namespace GrimoireTD.Map
{
    public interface IHexData
    {
        IHexType HexType { get; }

        IStructure StructureHere { get; }

        IUnit UnitHere { get; }

        IReadOnlyCallbackList<IDefenderAura> DefenderAurasHere { get; }

        //TODO remove (as also noted in CHexData)
        float pathingFScore { get; set; }
        float pathingGScore { get; set; }
        Coord pathingCameFrom { get; set; }

        void ResetPathingData();

        bool IsPathableByCreeps();

        bool IsPathableByCreepsWithTypePathable();

        bool IsPathableByCreepsWithStructureRemoved();

        bool IsPathableByCreepsWithUnitRemoved();

        bool CanPlaceStructureHere();

        bool CanPlaceUnitHere();

        bool IsEmpty();

        void AddStructureHere(IStructure structureAdded);

        void PlaceUnitHere(IUnit unitPlaced);

        void RemoveUnitHere();

        void AddDefenderAura(IDefenderAura aura);
    }
}