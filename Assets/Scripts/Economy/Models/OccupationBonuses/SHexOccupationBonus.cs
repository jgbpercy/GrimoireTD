using System;
using UnityEngine;
using GrimoireTD.Map;

namespace GrimoireTD.Economy
{
    [Serializable]
    public class SHexOccupationBonus : SOccupationBonus, IHexOccupationBonus
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