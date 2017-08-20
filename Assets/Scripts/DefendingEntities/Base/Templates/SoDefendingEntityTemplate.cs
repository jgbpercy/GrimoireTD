using UnityEngine;

namespace GrimoireTD.DefendingEntities
{
    public class SoDefendingEntityTemplate : ScriptableObject, IDefendingEntityTemplate
    {
        [SerializeField]
        private SoDefendingEntityImprovement baseCharacteristics;

        public IDefendingEntityImprovement BaseCharacteristics
        {
            get
            {
                return baseCharacteristics;
            }
        }
    }
}