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
    private SoHexType[] hexTypes;
    private Dictionary<BaseHexTypeEnum, IHexType> hexTypeDictionary;

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

    public ILevel Level
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
        hexTypeDictionary = new Dictionary<BaseHexTypeEnum, IHexType>();

        foreach (IHexType hexType in hexTypes)
        {
            hexTypeDictionary.Add(hexType.BaseHexType, hexType);
        }
    }

    private void ColorsToTypesArrayToDictionary()
    {
        colorsToTypesDictionary = new Dictionary<Color32, IHexType>();

        foreach (ColorToType colorToType in colorsToTypes)
        {
            colorsToTypesDictionary.Add(colorToType.color, hexTypeDictionary[colorToType.baseHexType]);
        }
    }

    public IHexType HexTypeFromName(BaseHexTypeEnum nameEnum)
    {
        return hexTypeDictionary[nameEnum];
    }

    private void PlaceStartingStructures()
    {
        foreach(StartingStructure startingStructure in level.StartingStructures)
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
        foreach(StartingUnit startingUnit in level.StartingUnits)
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
