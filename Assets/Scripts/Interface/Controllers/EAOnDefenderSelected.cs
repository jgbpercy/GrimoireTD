using System;
using GrimoireTD.Defenders.Structures;
using GrimoireTD.Defenders.Units;

namespace GrimoireTD.UI
{
    public class EAOnDefenderSelected : EventArgs
    {
        public readonly IStructure SelectedStructure;

        public readonly IUnit SelectedUnit;

        public EAOnDefenderSelected(IStructure selectedStructure, IUnit selectedUnit)
        {
            SelectedStructure = selectedStructure;
            SelectedUnit = selectedUnit;
        }
    }
}