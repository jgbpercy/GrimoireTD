using System;
using UnityEngine;

namespace GrimoireTD.Creeps
{
    [Serializable]
    public class SSpawn : ISpawn
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
}