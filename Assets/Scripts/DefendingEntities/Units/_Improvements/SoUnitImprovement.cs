using UnityEngine;
using System.Collections.Generic;
using GrimoireTD.Economy;

namespace GrimoireTD.DefendingEntities.Units
{
    [CreateAssetMenu(fileName = "NewUnitImprovement", menuName = "Structures and Units/Unit Improvement")]
    public class SoUnitImprovement : SoDefendingEntityImprovement, IUnitImprovement
    {
        [SerializeField]
        private HexOccupationBonus[] conditionalHexOccupationBonuses;

        [SerializeField]
        private StructureOccupationBonus[] conditionalStructureOccupationBonuses;

        public IEnumerable<HexOccupationBonus> ConditionalHexOccupationBonuses
        {
            get
            {
                return conditionalHexOccupationBonuses;
            }
        }

        public IEnumerable<StructureOccupationBonus> ConditionalStructureOccupationBonuses
        {
            get
            {
                return conditionalStructureOccupationBonuses;
            }
        }
    }
}