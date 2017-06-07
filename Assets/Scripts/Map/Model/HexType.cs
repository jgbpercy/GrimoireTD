using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewHexType", menuName = "Hexes/Hex Type")]
public class HexType : ScriptableObject {

    [SerializeField]
    private string nameInGame;

    [SerializeField]
    private BaseHexTypeEnum baseHexType;

    [SerializeField]
    private int[] textureOffset;

    //temporary
    [SerializeField]
    private bool isBuildable;

    [SerializeField]
    private bool isPathableByCreeps;

    [SerializeField]
    private bool unitCanOccupy;

    public string NameInGame
    {
        get
        {
            return nameInGame;
        }
    }

    public BaseHexTypeEnum BaseHexType
    {
        get
        {
            return baseHexType;
        }
    }

    public int[] TextureOffset
    {
        get
        {
            return textureOffset;
        }
    } 

    public bool IsBuildable
    {
        get
        {
            return isBuildable;
        }
    }

    public bool TypeIsPathableByCreeps
    {
        get
        {
            return isPathableByCreeps;
        }
    }

    public bool UnitCanOccupy
    {
        get
        {
            return unitCanOccupy;
        }
    }


}

public enum BaseHexTypeEnum
{
    GRASSLANDS,
    MOUNTAINS,
    WATER,
    DESERT,
    FOREST
}


