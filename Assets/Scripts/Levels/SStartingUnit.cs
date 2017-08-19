using System;
using UnityEngine;
using GrimoireTD.DefendingEntities.Units;
using GrimoireTD.Map;

namespace GrimoireTD.Levels
{
    [Serializable]
    public class SStartingUnit : IStartingUnit
    {
        [SerializeField]
        private SoUnitTemplate unitTemplate;
        [SerializeField]
        private Coord startingPosition;

        public IUnitTemplate UnitTemplate
        {
            get
            {
                return unitTemplate;
            }
        }

        public Coord StartingPosition
        {
            get
            {
                return startingPosition;
            }
        }
    }
}