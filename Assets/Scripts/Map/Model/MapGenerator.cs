using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ColorToType
{
    public Color32 color;
    public BaseHexTypeEnum baseHexType;
}

public class MapGenerator : SingletonMonobehaviour<MapGenerator> {

    private MapData map;

    [SerializeField]
    private HexType[] hexTypes;
    private Dictionary<BaseHexTypeEnum, HexType> hexTypeDictionary;

    [SerializeField]
    private Level level;

    [SerializeField]
    private ColorToType[] colorsToTypes;
    private Dictionary<Color32, HexType> colorsToTypesDictionary;

    public MapData Map
    {
        get
        {
            return map;
        }
    }

    public Level Level
    {
        get
        {
            return level;
        }
    }

    private void Awake()
    {
        BuildHexTypesDictionary();
        ColorsToTypesArrayToDictionary();
    }

    private void Start()
    {
        CDebug.Log(CDebug.applicationLoading, "Map Generator Start");

        map = new MapData(level.LevelImage, colorsToTypesDictionary);

        PlaceStartingStructures();
        PlaceStartingUnits();
    }

    private void BuildHexTypesDictionary()
    {
        hexTypeDictionary = new Dictionary<BaseHexTypeEnum, HexType>();

        foreach (HexType hexType in hexTypes)
        {
            hexTypeDictionary.Add(hexType.BaseHexType, hexType);
        }
    }

    private void ColorsToTypesArrayToDictionary()
    {
        colorsToTypesDictionary = new Dictionary<Color32, HexType>();

        foreach (ColorToType colorToType in colorsToTypes)
        {
            colorsToTypesDictionary.Add(colorToType.color, hexTypeDictionary[colorToType.baseHexType]);
        }
    }

    public HexType HexTypeFromName(BaseHexTypeEnum nameEnum)
    {
        return hexTypeDictionary[nameEnum];
    }

    private void PlaceStartingStructures()
    {
        foreach(Level.StartingStructure startingStructure in level.StartingStructures)
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
        foreach(Level.StartingUnit startingUnit in level.StartingUnits)
        {
            if (!map.CanCreateUnitAt(startingUnit.StartingPosition) )
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
