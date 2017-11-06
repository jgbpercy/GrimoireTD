using System;
using GrimoireTD.Defenders.Structures;

namespace GrimoireTD.UI
{
    public class EAOnStructureToBuildSelected : EventArgs
    {
        public readonly IStructureTemplate SelectedStructureTemplate;

        public EAOnStructureToBuildSelected(IStructureTemplate selectedStructureTemplate)
        {
            SelectedStructureTemplate = selectedStructureTemplate;
        }
    }
}