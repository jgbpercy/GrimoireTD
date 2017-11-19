using UnityEngine;
using System.Collections.Generic;
using GrimoireTD.Economy;

namespace GrimoireTD.Defenders.Units
{
    [CreateAssetMenu(fileName = "NewUnitImprovement", menuName = "Structures and Units/Unit Improvement")]
    public class SoUnitImprovement : SoDefenderImprovement, IUnitImprovement
    {
        [SerializeField]
        private SHexOccupationBonus[] conditionalHexOccupationBonuses;

        [SerializeField]
        private SStructureOccupationBonus[] conditionalStructureOccupationBonuses;

        public ICollection<IHexOccupationBonus> ConditionalHexOccupationBonuses
        {
            get
            {
                return conditionalHexOccupationBonuses;
            }
        }

        public ICollection<IStructureOccupationBonus> ConditionalStructureOccupationBonuses
        {
            get
            {
                return conditionalStructureOccupationBonuses;
            }
        }
    }
}