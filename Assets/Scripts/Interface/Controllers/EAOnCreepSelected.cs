using System;
using GrimoireTD.Creeps;

namespace GrimoireTD.UI
{
    public class EAOnCreepSelected : EventArgs
    {
        public readonly ICreep SelectedCreep;

        public EAOnCreepSelected(ICreep selectedCreep)
        {
            SelectedCreep = selectedCreep;
        }
    }
}