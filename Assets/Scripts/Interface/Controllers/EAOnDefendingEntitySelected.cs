using System;
using GrimoireTD.DefendingEntities.Structures;
using GrimoireTD.DefendingEntities.Units;

namespace GrimoireTD.UI
{
    public class EAOnDefendingEntitySelected : EventArgs
    {
        public readonly IStructure SelectedStructure;

        public readonly IUnit SelectedUnit;

        public EAOnDefendingEntitySelected(IStructure selectedStructure, IUnit selectedUnit)
        {
            SelectedStructure = selectedStructure;
            SelectedUnit = selectedUnit;
        }
    }
}