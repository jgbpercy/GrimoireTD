using System;
using UnityEngine;

namespace GrimoireTD.DefendingEntities
{
    [Serializable]
    public class DefendingEntityGraphicsMapping
    {
        [SerializeField]
        private SoDefendingEntityTemplate defendingEntityTemplate;

        [SerializeField]
        private GameObject prefab;

        public SoDefendingEntityTemplate DefendingEntityTemplate
        {
            get
            {
                return defendingEntityTemplate;
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