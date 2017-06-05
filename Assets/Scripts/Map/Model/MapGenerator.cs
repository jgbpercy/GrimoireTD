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

    private Dictionary<Color32, HexType> colorsToTypesDictionary;

    [SerializeField]
    private HexType[] hexTypes;

    private Dictionary<BaseHexTypeEnum, HexType> hexTypeDictionary;

    [SerializeField]
    private Level level;

    [SerializeField]
    private ColorToType[] colorsToTypes;

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

        map.GeneratePath();
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
            if (!map.TryBuildStructureAtFree(startingStructure.StartingPosition, startingStructure.StructureTemplate))
            {
                throw new Exception("Attempted to add Starting Structure at invalid position (" + startingStructure.StartingPosition.X + ", " + startingStructure.StartingPosition.Y + ")");
            }
        }
    }

    private void PlaceStartingUnits()
    {
        foreach(Level.StartingUnit startingUnit in level.StartingUnits)
        {
            if (!map.TryCreateUnitAt(startingUnit.StartingPosition, startingUnit.UnitTemplate) )
            {
                throw new Exception("Attempted to add Starting Unit at invalid position (" + startingUnit.StartingPosition.X + ", " + startingUnit.StartingPosition.Y + ")");
            }
        }
    }
}
