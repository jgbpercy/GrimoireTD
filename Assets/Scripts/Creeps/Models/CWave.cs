using System.Collections.Generic;

namespace GrimoireTD.Creeps
{
    public class CWave : IWave
    {
        private SortedList<float, ICreepTemplate> spawns;

        public IReadOnlyDictionary<float, ICreepTemplate> Spawns
        {
            get
            {
                return spawns;
            }
        }

        public CWave(IWaveTemplate template)
        {
            spawns = new SortedList<float, ICreepTemplate>();

            float previousTiming = 0;

            for (int spawnIndex = 0; spawnIndex < template.Spawns.Count; spawnIndex++)
            {
                var thisTiming = template.Spawns[spawnIndex].Timing + previousTiming;

                spawns.Add(
                    thisTiming,
                    template.Spawns[spawnIndex].Creep
                );

                previousTiming = thisTiming;
            }
        }

        /* REMEMBER that timings are in the serialised UI object as offset from previous (e.g. 0.25, 0.25, 0.25 etc)
         * BUT they are in the model data as timings into the wave (e.g. 0.25, 0.5, 0.75)
         * This conversion in done in the constructor above
         * This makes the entry more maintainable (you don't have to update all subsequent entries when changing one near the start)
         * And make the model data make sense because you want unique timings in the sorted list
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

        //Caller responsible for checking CreepsRemaining
        public ICreepTemplate DequeueNextCreep()
        {
            ICreepTemplate returnCreep = spawns.Values[0];

            spawns.RemoveAt(0);

            return returnCreep;
        }
    }
}