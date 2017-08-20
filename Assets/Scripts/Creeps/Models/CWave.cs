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

        /* REMEMBER that timings are in the serialised UI object as offset from previous (e.g. 0.25, 0.25, 0.25 etc)
         * BUT they are in the model data as timings into the wave (e.g. 0.25, 0.5, 0.75)
         * This makes the entry more maintainable (you don't have to update all subsequent entries when changing one near the start)
         * And make the model data make sense because you want unique timings in a sorted list
         */
        public float NextSpawnTime()
        {
            if (spawns.Count == 0)
            {
                return 0f;
            }
            return spawns.Keys[0];
        }

        public bool CreepsRemaining()
        {
            return spawns.Count != 0;
        }

        public ICreepTemplate DequeueNextCreep()
        {
            ICreepTemplate returnCreep = spawns.Values[0];

            spawns.RemoveAt(0);

            return returnCreep;
        }
    }
}