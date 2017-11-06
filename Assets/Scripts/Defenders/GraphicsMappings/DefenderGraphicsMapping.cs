using System;
using UnityEngine;

namespace GrimoireTD.Defenders
{
    [Serializable]
    public class DefenderGraphicsMapping
    {
        [SerializeField]
        private SoDefenderTemplate defenderTemplate;

        [SerializeField]
        private GameObject prefab;

        public SoDefenderTemplate DefenderTemplate
        {
            get
            {
                return defenderTemplate;
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