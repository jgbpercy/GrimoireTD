using System.Collections.Generic;
using UnityEngine;

namespace GrimoireTD.DefendingEntities.Units
{
    [CreateAssetMenu(fileName = "NewUnitTalent", menuName = "Structures and Units/Unit Talent")]
    public class SoUnitTalent : ScriptableObject, IUnitTalent
    {
        [SerializeField]
        private SoUnitImprovement[] unitImprovements;

        [SerializeField]
        private string descriptionText;

        public IReadOnlyList<IUnitImprovement> UnitImprovements
        {
            get
            {
                return unitImprovements;
            }
        }

        public string DescriptionText
        {
            get
            {
                return descriptionText;
            }
        }
    }
}