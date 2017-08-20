using System;
using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    [Serializable]
    public class ProjectileGraphicsMapping
    {
        [SerializeField]
        private SoProjectileTemplate projectileTemplate;

        [SerializeField]
        private GameObject prefab;

        public ProjectileGraphicsMapping()
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