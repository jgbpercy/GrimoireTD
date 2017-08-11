using UnityEngine;

namespace GrimoireTD.DefendingEntities
{
    public class SoDefendingEntityTemplate : ScriptableObject, IDefendingEntityTemplate
    {
        [SerializeField]
        private SoDefendingEntityImprovement baseCharacteristics;

        [SerializeField]
        protected GameObject prefab;

        public IDefendingEntityImprovement BaseCharacteristics
        {
            get
            {
                return baseCharacteristics;
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