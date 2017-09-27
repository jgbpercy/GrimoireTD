using System;
using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    [Serializable]
    public class SProjectileGraphicsMapping
    {
        [SerializeField]
        private SoProjectileTemplate projectileTemplate;

        [SerializeField]
        private GameObject prefab;

        public SProjectileGraphicsMapping()
        {
        }

        public SoProjectileTemplate ProjectileTemplate
        {
            get
            {
                return projectileTemplate;
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