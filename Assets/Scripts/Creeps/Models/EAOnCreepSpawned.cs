using System;

namespace GrimoireTD.Creeps
{
    public class EAOnCreepSpawned : EventArgs
    {
        public readonly ICreep NewCreep;

        public EAOnCreepSpawned(ICreep newCreep)
        {
            NewCreep = newCreep;
        }
    }
}