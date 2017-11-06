using System.Collections.Generic;
using UnityEngine;

namespace GrimoireTD.Defenders.Structures
{
    [CreateAssetMenu(fileName = "NewStuctureUpgrade", menuName = "Structures and Units/Structure Upgrade")]
    public class SoStructureUpgrade : ScriptableObject, IStructureUpgrade
    {
        [SerializeField]
        private SStructureEnhancement[] optionalEnhancements;

        [SerializeField]
        private SoDefenderImprovement mainUpgradeBonus;

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

        public IDefenderImprovement MainUpgradeBonus
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