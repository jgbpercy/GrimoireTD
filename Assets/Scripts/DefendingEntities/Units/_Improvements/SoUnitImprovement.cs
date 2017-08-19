using UnityEngine;
using System.Collections.Generic;
using GrimoireTD.Economy;

namespace GrimoireTD.DefendingEntities.Units
{
    [CreateAssetMenu(fileName = "NewUnitImprovement", menuName = "Structures and Units/Unit Improvement")]
    public class SoUnitImprovement : SoDefendingEntityImprovement, IUnitImprovement
    {
        [SerializeField]
        private SHexOccupationBonus[] conditionalHexOccupationBonuses;

        [SerializeField]
        private SStructureOccupationBonus[] conditionalStructureOccupationBonuses;

        public IEnumerable<IHexOccupationBonus> ConditionalHexOccupationBonuses
        {
            get
            {
                return conditionalHexOccupationBonuses;
            }
        }

        public IEnumerable<IStructureOccupationBonus> ConditionalStructureOccupationBonuses
        {
            get
            {
                return conditionalStructureOccupationBonuses;
            }
        }
    }
}