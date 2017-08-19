using System.Collections.Generic;
using UnityEngine;

namespace GrimoireTD.DefendingEntities.Structures
{
    [CreateAssetMenu(fileName = "NewStuctureUpgrade", menuName = "Structures and Units/Structure Upgrade")]
    public class SoStructureUpgrade : ScriptableObject, IStructureUpgrade
    {
        [SerializeField]
        private SStructureEnhancement[] optionalEnhancements;

        [SerializeField]
        private SoDefendingEntityImprovement mainUpgradeBonus;

        [SerializeField]
        private string newStructureName;

        [SerializeField]
        private string newStructureDescription;

        [SerializeField]
        private string bonusDescription;

        public IEnumerable<IStructureEnhancement> OptionalEnhancements
        {
            get
            {
                return optionalEnhancements;
            }
        }

        public IDefendingEntityImprovement MainUpgradeBonus
        {
            get
            {
                return mainUpgradeBonus;
            }
        }

        public string NewStructureName
        {
            get
            {
                return newStructureName;
            }
        }

        public string NewStructureDescription
        {
            get
            {
                return newStructureDescription;
            }
        }

        public string BonusDescription
        {
            get
            {
                return bonusDescription;
            }
        }
    }
}