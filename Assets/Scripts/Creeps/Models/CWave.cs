using System.Collections.Generic;

namespace GrimoireTD.Creeps
{
    public class CWave : IWave
    {

        private SortedList<float, ICreepTemplate> spawns;

        private bool debugOn;

        public IReadOnlyDictionary<float, ICreepTemplate> Spawns
        {
            get
            {
                return spawns;
            }
        }

        public CWave()
        {
            spawns = new SortedList<float, ICreepTemplate>();
        }

        public CWave(SortedList<float, ICreepTemplate> spawns)
        {
            this.spawns = spawns;
        }

        public CWave(float[] timings, ICreepTemplate[] creeps)
        {
            spawns = new SortedList<float, ICreepTemplate>();

            for (int i = 0; i < timings.Length; i++)
            {
                if (creeps.Length > i)
                {
                    spawns.Add(timings[i], creeps[i]);
                }
            }
        }

        public float NextSpawnTime()
        {
            if (spawns.Count == 0)
            {
                return 0f;
            }
            return spawns.Keys[0];
        }

        public ICreepTemplate SpawnNextCreep()
        {
            ICreepTemplate returnCreep = spawns.Values[0];

            spawns.RemoveAt(0);

            return returnCreep;
        }
    }
}