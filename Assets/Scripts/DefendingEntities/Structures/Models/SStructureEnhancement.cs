using UnityEngine;
using System;
using GrimoireTD.Economy;

namespace GrimoireTD.DefendingEntities.Structures
{
    //TODO: Make SO/I?
    [Serializable]
    public class SStructureEnhancement : IStructureEnhancement
    {
        [SerializeField]
        private SoDefendingEntityImprovement enhancementBonus;

        [SerializeField]
        private string descriptionText;

        [SerializeField]
        private SEconomyTransaction cost;

        public IDefendingEntityImprovement EnhancementBonus
        {
            get
            {
                return enhancementBonus;
            }
        }

        public string DescriptionText
        {
            get
            {
                return descriptionText;
            }
        }

        public IEconomyTransaction Cost
        {
            get
            {
                return cost;
            }
        }
    }
}