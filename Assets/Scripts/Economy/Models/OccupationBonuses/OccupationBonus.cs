using System;
using UnityEngine;

namespace GrimoireTD.Economy
{
    [Serializable]
    public abstract class OccupationBonus
    {
        [SerializeField]
        private SEconomyTransaction resourceGain;

        public IEconomyTransaction ResourceGain
        {
            get
            {
                return resourceGain;
            }
        }
    }
}