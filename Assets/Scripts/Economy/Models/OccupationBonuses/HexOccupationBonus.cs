using System;
using UnityEngine;
using GrimoireTD.Map;

namespace GrimoireTD.Economy
{
    [Serializable]
    public class HexOccupationBonus : OccupationBonus
    {
        [SerializeField]
        private SoHexType hexType;

        public IHexType HexType
        {
            get
            {
                return hexType;
            }
        }
    }
}