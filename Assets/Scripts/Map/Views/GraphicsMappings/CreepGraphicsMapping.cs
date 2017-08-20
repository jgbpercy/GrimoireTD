using System;
using UnityEngine;

namespace GrimoireTD.Creeps
{
    [Serializable]
    public class CreepGraphicsMapping
    {
        [SerializeField]
        private SoCreepTemplate creepTemplate;

        [SerializeField]
        private GameObject prefab;

        public ICreepTemplate CreepTemplate
        {
            get
            {
                return creepTemplate;
            }
        }

        public GameObject Prefab
        {
            get
            {
                return prefab;
            }
        }
    }
}