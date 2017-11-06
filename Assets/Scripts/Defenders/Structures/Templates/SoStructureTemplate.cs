using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Economy;
using GrimoireTD.Map;

namespace GrimoireTD.Defenders.Structures
{
    [CreateAssetMenu(fileName = "NewStructure", menuName = "Structures and Units/Structure")]
    public class SoStructureTemplate : SoDefenderTemplate, IStructureTemplate
    {
        [SerializeField]
        private string startingNameInGame;

        [SerializeField]
        private string startingDescription;

        [SerializeField]
        private SEconomyTransaction cost;

        [SerializeField]
        private SoStructureUpgrade[] structureUpgrades;

        public string StartingNameInGame
        {
            get
            {
                return startingNameInGame;
            }
        }

        public string StartingDescription
        {
            get
            {
                return startingDescription;
            }
        }

        public IEconomyTransaction Cost
        {
            get
            {
                return cost;
            }
        }

        public IEnumerable<IStructureUpgrade> StructureUpgrades
        {
            get
            {
                return structureUpgrades;
            }
        }

        public string UIText()
        {
            string uiText = Cost.ToString(EconomyTransactionStringFormat.ShortNameSingleLine, true) + "\n";

            return uiText;
        }

        public virtual IStructure GenerateStructure(Coord position)
        {
            return new CStructure(this, position);
        }
    }
}