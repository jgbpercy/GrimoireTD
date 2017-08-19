using System;
using System.Collections.Generic;
using GrimoireTD.DefendingEntities;
using GrimoireTD.DefendingEntities.Structures;
using GrimoireTD.DefendingEntities.Units;

namespace GrimoireTD.Map
{
    public interface IMapData
    {
        int Width { get; }
        int Height { get; }

        IReadOnlyList<Coord> CreepPath { get; }

        IHexData TryGetHexAt(Coord hexCoord);
        IHexData GetHexAt(Coord hexCoord);

        List<Coord> GetNeighboursOf(Coord hexCoord);

        bool CanBuildGenericStructureAt(Coord coord);

        bool CanBuildStructureAt(Coord coord, IStructureTemplate structureTemplate, bool isFree);

        bool TryBuildStructureAt(Coord coord, IStructureTemplate structureTemplate, bool isFree);

        bool CanCreateUnitAt(Coord coord);

        bool TryCreateUnitAt(Coord targetCoord, IUnitTemplate unitTemplate);

        bool CanMoveUnitTo(Coord targetCoord, IReadOnlyList<Coord> disallowedCoordsForMove);

        bool TryMoveUnitTo(Coord fromCoord, Coord targetCoord, IUnit unit, List<Coord> disallowedCoordsForMove);

        Coord WhereAmI(IDefendingEntity defendingEntity);

        List<Coord> GetDisallowedCoordsAfterUnitMove(Coord fromCoord);

        void RegisterForOnPathGeneratedOrChangedCallback(Action callback);
        void DeregisterForOnPathGeneratedOrChangedCallback(Action callback);
    }
}