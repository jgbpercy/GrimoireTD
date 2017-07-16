using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Levels/Level")]
public class Level : ScriptableObject {

    [Serializable]
    public class StartingUnit
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

    [Serializable]
    public class StartingStructure
    {
        [SerializeField]
        private SoStructureTemplate structureTemplate;
        [SerializeField]
        private Coord startingPosition;

        public IStructureTemplate StructureTemplate
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
    private SoWaveTemplate[] waves;

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

    public IWaveTemplate[] Waves
    {
        get {
            return waves;
        }
    }
}
