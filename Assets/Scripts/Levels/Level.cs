using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Levels/Level")]
public class Level : ScriptableObject {

    [Serializable]
    public class StartingUnit
    {
        [SerializeField]
        private UnitTemplate unitTemplate;
        [SerializeField]
        private Coord startingPosition;

        public UnitTemplate UnitTemplate
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

    [Serializable]
    public class StartingStructure
    {
        [SerializeField]
        private StructureTemplate structureTemplate;
        [SerializeField]
        private Coord startingPosition;

        public StructureTemplate StructureTemplate
        {
            get
            {
                return structureTemplate;
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

    //level layout
    [SerializeField]
    private Texture2D levelImage;

    //starting resources
    [SerializeField]
    private EconomyTransaction startingResources;

    //starting units
    [SerializeField]
    private StartingUnit[] startingUnits;

    //starting structures
    [SerializeField]
    private StartingStructure[] startingStructures;

    //waves
    [SerializeField]
    private WaveTemplate[] waves;

    public Texture2D LevelImage
    {
        get
        {
            return levelImage;
        }
    }

    public EconomyTransaction StartingResources
    {
        get
        {
            return startingResources;
        }
    }

    public StartingUnit[] StartingUnits
    {
        get
        {
            return startingUnits;
        }
    }

    public StartingStructure[] StartingStructures
    {
        get
        {
            return startingStructures;
        }
    }

    public WaveTemplate[] Waves
    {
        get {
            return waves;
        }
    }
}
