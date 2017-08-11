using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Creeps;
using GrimoireTD.Economy;

namespace GrimoireTD.Levels
{
    [CreateAssetMenu(fileName = "NewLevel", menuName = "Levels/Level")]
    public class SoLevel : ScriptableObject, ILevel
    {
        [SerializeField]
        private Texture2D levelImage;

        [SerializeField]
        private SEconomyTransaction startingResources;

        [SerializeField]
        private StartingUnit[] startingUnits;

        [SerializeField]
        private StartingStructure[] startingStructures;

        [SerializeField]
        private SoWaveTemplate[] waves;

        public Texture2D LevelImage
        {
            get
            {
                return levelImage;
            }
        }

        public IEconomyTransaction StartingResources
        {
            get
            {
                return startingResources;
            }
        }

        public IEnumerable<StartingUnit> StartingUnits
        {
            get
            {
                return startingUnits;
            }
        }

        public IEnumerable<StartingStructure> StartingStructures
        {
            get
            {
                return startingStructures;
            }
        }

        public IEnumerable<IWaveTemplate> Waves
        {
            get
            {
                return waves;
            }
        }
    }
}