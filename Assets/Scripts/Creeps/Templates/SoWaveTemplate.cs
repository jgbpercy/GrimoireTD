using UnityEngine;
using System;

namespace GrimoireTD.Creeps
{
    [CreateAssetMenu(fileName = "NewWave", menuName = "Levels/Wave")]
    public class SoWaveTemplate : ScriptableObject, IWaveTemplate
    {
        [Serializable]
        public class Spawn
        {
            [SerializeField]
            private float timing;
            [SerializeField]
            private SoCreepTemplate creep;

            public float Timing
            {
                get
                {
                    return timing;
                }
            }

            public ICreepTemplate Creep
            {
                get
                {
                    return creep;
                }
            }
        }

        [SerializeField]
        private Spawn[] spawns;

        public IWave GenerateWave()
        {
            float[] timings = new float[spawns.Length];
            ICreepTemplate[] creeps = new ICreepTemplate[spawns.Length];

            for (int i = 0; i < spawns.Length; i++)
            {
                timings[i] = i == 0 ? spawns[i].Timing : spawns[i].Timing + timings[i - 1];
                creeps[i] = spawns[i].Creep;
            }

            return new CWave(timings, creeps);
        }
    }
}