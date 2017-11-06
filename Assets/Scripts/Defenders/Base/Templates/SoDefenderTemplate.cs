using UnityEngine;

namespace GrimoireTD.Defenders
{
    public class SoDefenderTemplate : ScriptableObject, IDefenderTemplate
    {
        [SerializeField]
        private SoDefenderImprovement baseCharacteristics;

        public IDefenderImprovement BaseCharacteristics
        {
            get
            {
                return baseCharacteristics;
            }
        }
    }
}