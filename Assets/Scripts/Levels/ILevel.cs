using System.Collections.Generic;
using UnityEngine;

public interface ILevel {

    Texture2D LevelImage { get; }

    IEconomyTransaction StartingResources { get; }

    IEnumerable<StartingUnit> StartingUnits { get; }

    IEnumerable<StartingStructure> StartingStructures { get; }

    IEnumerable<IWaveTemplate> Waves { get; }
}
