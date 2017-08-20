using System;
using System.Collections.Generic;
using GrimoireTD.DefendingEntities;
using GrimoireTD.DefendingEntities.Structures;
using GrimoireTD.DefendingEntities.Units;

namespace GrimoireTD.Map
{
    public interface IReadOnlyMapData
    {
        int Width { get; }
        int Height { get; }

        IReadOnlyList<IHexType> HexTypes { get; }

        IReadOnlyList<Coord> CreepPath { get; }

        IHexData TryGetHexAt(Coord hexCoord);
        IHexData GetHexAt(Coord hexCoord);

        List<Coord> GetNeighboursOf(Coord hexCoord);

        bool CanBuildStructureAt(Coord coord);

        bool CanCreateUnitAt(Coord coord);

        bool CanMoveUnitTo(Coord targetCoord, IReadOnlyList<Coord> disallowedCoordsForMove);

        Coord WhereAmI(IDefendingEntity defendingEntity);

        List<Coord> CoordsInRange(int range, Coord startHex);

        List<Coord> GetDisallowedCoordsAfterUnitMove(Coord fromCoord);

        void RegisterForOnPathGeneratedOrChangedCallback(Action callback);
        void DeregisterForOnPathGeneratedOrChangedCallback(Action callback);

        void RegisterForOnMapCreatedCallback(Action callback);
        void DeregisterForOnMapCreatedCallback(Action callback);

        void RegisterForOnUnitCreatedCallback(Action<IUnit, Coord> callback);
        void DeregisterForOnUnitCreatedCallback(Action<IUnit, Coord> callback);

        void RegisterForOnStructureCreatedCallback(Action<IStructure, Coord> callback);
        void DeregisterForOnStructureCreatedCallback(Action<IStructure, Coord> callback);
    }
}