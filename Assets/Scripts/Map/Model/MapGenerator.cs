using System;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Levels;
using GrimoireTD.Technical;
using GrimoireTD.ChannelDebug;

namespace GrimoireTD.Map
{
    [Serializable]
    public class ColorToType
    {
        [SerializeField]
        private Color32 color;

        [SerializeField]
        public SoHexType hexType;

        public Color32 Color
        {
            get
            {
                return color;
            }
        }

        public IHexType HexType
        {
            get
            {
                return hexType;
            }
        }
    }

    public class MapGenerator : SingletonMonobehaviour<MapGenerator>
    {
        private MapData map;

        [SerializeField]
        private SoHexType[] hexTypes;

        /*TODO: Make this fit the template/generator pattern better so that MapGenerator (rename) is a pure 
         * class with a constructor not a singleton monobehaviour and something else bootstraps the level load?
         * */
        [SerializeField]
        private SoLevel level;

        [SerializeField]
        private ColorToType[] colorsToTypes;
        private Dictionary<Color32, IHexType> colorsToTypesDictionary;

        public MapData Map
        {
            get
            {
                return map;
            }
        }

        public IReadOnlyCollection<IHexType> HexTypes
        {
            get
            {
                return hexTypes;
            }
        }

        public ILevel Level
        {
            get
            {
                return level;
            }
        }

        private void Awake()
        {
            ColorsToTypesArrayToDictionary();
        }

        private void Start()
        {
            CDebug.Log(CDebug.applicationLoading, "Map Generator Start");

            map = new MapData(level.LevelImage, colorsToTypesDictionary);

            PlaceStartingStructures();
            PlaceStartingUnits();
        }

        private void ColorsToTypesArrayToDictionary()
        {
            colorsToTypesDictionary = new Dictionary<Color32, IHexType>();

            foreach (ColorToType colorToType in colorsToTypes)
            {
                colorsToTypesDictionary.Add(colorToType.Color, colorToType.hexType);
            }
        }

        private void PlaceStartingStructures()
        {
            foreach (StartingStructure startingStructure in level.StartingStructures)
            {
                if (!map.CanBuildStructureAt(startingStructure.StartingPosition, startingStructure.StructureTemplate, true))
                {
                    throw new Exception("Attempted to add Starting Structure at invalid position (" + startingStructure.StartingPosition.X + ", " + startingStructure.StartingPosition.Y + ")");
                }
                else
                {
                    map.TryBuildStructureAt(startingStructure.StartingPosition, startingStructure.StructureTemplate, true);
                }
            }
        }

        private void PlaceStartingUnits()
        {
            foreach (StartingUnit startingUnit in level.StartingUnits)
            {
                if (!map.CanCreateUnitAt(startingUnit.StartingPosition))
                {
                    throw new Exception("Attempted to add Starting Unit at invalid position (" + startingUnit.StartingPosition.X + ", " + startingUnit.StartingPosition.Y + ")");
                }
                else
                {
                    map.TryCreateUnitAt(startingUnit.StartingPosition, startingUnit.UnitTemplate);
                }
            }
        }
    }
}