using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ColorToType
{
    public Color32 color;
    public HexType hexType;

}

public class MapGenerator : SingletonMonobehaviour<MapGenerator> {

    private MapData map;

    private Dictionary<Color32, HexType> colorsToTypesDictionary;

    [SerializeField]
    private Texture2D levelImage;

    [SerializeField]
    private ColorToType[] colorsToTypes;

    public MapData Map
    {
        get
        {
            return map;
        }
    }

    private void Awake()
    {
        colorsToTypesArrayToDictionary();
    }

    private void Start()
    {
        CDebug.Log(CDebug.applicationLoading, "Map Generator Start");

        map = new MapData(levelImage, colorsToTypesDictionary);

        map.GeneratePath();
    }

    private void colorsToTypesArrayToDictionary()
    {
        colorsToTypesDictionary = new Dictionary<Color32, HexType>();

        foreach (ColorToType colorToType in colorsToTypes)
        {
            colorsToTypesDictionary.Add(colorToType.color, colorToType.hexType);
        }
    }


}
