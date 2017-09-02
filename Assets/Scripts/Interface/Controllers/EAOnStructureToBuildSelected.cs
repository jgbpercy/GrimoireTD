using System;
using GrimoireTD.DefendingEntities.Structures;

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