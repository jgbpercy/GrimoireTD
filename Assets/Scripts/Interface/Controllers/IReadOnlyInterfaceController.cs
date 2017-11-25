using System;
using GrimoireTD.Map;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Defenders.Structures;
using GrimoireTD.Defenders.Units;

namespace GrimoireTD.UI
{
    public interface IReadOnlyInterfaceController : IModelInterfaceController
    {
        InterfaceCursorMode CurrentCursorMode { get; }

        Coord MouseOverCoord { get; }
        IHexData MouseOverHex { get; }
        bool MouseRaycastHitMap { get; }

        IPlayerTargetedComponent SelectedBuildModeAbilityTargetingComponent { get; }
        IStructure SelectedStructureInstance { get; }
        IUnit SelectedUnitInstance { get; }
        Coord SelectedUnitLocation { get; }

        event EventHandler<EAOnStructureToBuildDeselected> OnStructureToBuildDeselected;
        event EventHandler<EAOnStructureToBuildSelected> OnStructureToBuildSelected;
    }
}