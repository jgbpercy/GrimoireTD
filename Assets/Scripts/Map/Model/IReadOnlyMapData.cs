﻿using System;
using System.Collections.Generic;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Map
{
    public interface IReadOnlyMapData
    {
        int Width { get; }
        int Height { get; }

        IReadOnlyList<IHexType> HexTypes { get; }

        IReadOnlyList<Coord> CreepPath { get; }

        event EventHandler<EAOnMapCreated> OnMapCreated;
        event EventHandler<EAOnPathGeneratedOrChanged> OnPathGeneratedOrChanged;

        event EventHandler<EAOnUnitCreated> OnUnitCreated;
        event EventHandler<EAOnStructureCreated> OnStructureCreated;

        IHexData TryGetHexAt(Coord hexCoord);
        IHexData GetHexAt(Coord hexCoord);

        List<Coord> GetNeighboursOf(Coord hexCoord);

        bool CanBuildStructureAt(Coord coord);

        bool CanCreateUnitAt(Coord coord);

        bool CanMoveUnitTo(Coord targetCoord, IReadOnlyList<Coord> disallowedCoordsForMove);

        Coord WhereAmI(IDefendingEntity defendingEntity);

        List<Coord> CoordsInRange(int range, Coord startHex);

        List<Coord> GetDisallowedCoordsAfterUnitMove(Coord fromCoord);
    }
}