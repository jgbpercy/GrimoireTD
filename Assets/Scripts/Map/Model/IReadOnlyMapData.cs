using System;
using System.Collections.Generic;
using GrimoireTD.Defenders;

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

        List<Coord> GetExtantNeighboursOf(Coord hexCoord);

        bool CanBuildStructureAt(Coord coord);

        bool CanCreateUnitAt(Coord coord);

        bool CanMoveUnitTo(Coord targetCoord, IReadOnlyList<Coord> disallowedCoordsForMove);

        List<Coord> GetCoordsInRange(int range, Coord startHex);

        List<Coord> GetDisallowedMovementDestinationCoords(Coord fromCoord);
    }
}