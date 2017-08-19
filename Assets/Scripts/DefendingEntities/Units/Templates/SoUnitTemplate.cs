using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Map;

namespace GrimoireTD.DefendingEntities.Units
{
    [CreateAssetMenu(fileName = "NewUnit", menuName = "Structures and Units/Unit")]
    public class SoUnitTemplate : SoDefendingEntityTemplate, IUnitTemplate
    {
        [SerializeField]
        private string nameInGame;

        [SerializeField]
        private string description;

        [SerializeField]
        private int experienceToLevelUp = 100;

        [SerializeField]
        private SoUnitTalent[] unitTalents;

        public string NameInGame
        {
            get
            {
                return nameInGame;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
        }

        public int ExperienceToLevelUp
        {
            get
            {
                return experienceToLevelUp;
            }
        }

        public IEnumerable<IUnitTalent> UnitTalents
        {
            get
            {
                return unitTalents;
            }
        }

        public IUnitImprovement BaseUnitCharacteristics
        {
            get
            {
                SoUnitImprovement baseUnitCharacteristics = BaseCharacteristics as SoUnitImprovement;
                if (baseUnitCharacteristics != null)
                {
                    return baseUnitCharacteristics;
                }
                throw new System.Exception("SoUnitTemplate BaseCharacteristics is not a SoUnitImprovement");
            }
        }

        public virtual IUnit GenerateUnit(Coord position)
        {
            return new CUnit(this, position);
        }
    }
}