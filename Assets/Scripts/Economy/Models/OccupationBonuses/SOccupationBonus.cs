using System;
using UnityEngine;

namespace GrimoireTD.Economy
{
    [Serializable]
    public abstract class SOccupationBonus : IOccupationBonus
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