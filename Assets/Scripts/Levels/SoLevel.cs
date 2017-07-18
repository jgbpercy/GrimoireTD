using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Levels/Level")]
public class SoLevel : ScriptableObject, ILevel {

    [SerializeField]
    private Texture2D levelImage;

    [SerializeField]
    private EconomyTransaction startingResources;

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

    public EconomyTransaction StartingResources
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
        get {
            return waves;
        }
    }
}
