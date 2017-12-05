using System.Collections.Generic;
using GrimoireTD.Defenders.DefenderEffects;
using GrimoireTD.Defenders.Structures;
using GrimoireTD.Defenders.Units;

namespace GrimoireTD.Map
{
    public interface IHexData
    {
        IHexType HexType { get; }

        IStructure StructureHere { get; }

        IUnit UnitHere { get; }

        IReadOnlyCallbackList<IDefenderAura> DefenderAurasHere { get; }

        bool IsPathableByCreeps();

        bool IsPathableByCreepsWithTypePathable();

        bool IsPathableByCreepsWithStructureRemoved();

        bool IsPathableByCreepsWithUnitRemoved();

        bool CanBuildStructureHere();

        bool CanPlaceUnitHere();

        bool IsEmpty();

        void BuildStructureHere(IStructure structureAdded);

        void PlaceUnitHere(IUnit unitPlaced);

        void RemoveUnitHere();

        void AddDefenderAura(IDefenderAura aura);
    }
}