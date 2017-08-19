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
        private SStartingUnit[] startingUnits;

        [SerializeField]
        private SStartingStructure[] startingStructures;

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

        public IEnumerable<IStartingUnit> StartingUnits
        {
            get
            {
                return startingUnits;
            }
        }

        public IEnumerable<IStartingStructure> StartingStructures
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