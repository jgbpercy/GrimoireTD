using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Creeps;
using GrimoireTD.Economy;

namespace GrimoireTD.Levels
{
    public interface ILevel
    {
        Texture2D LevelImage { get; }

        IEconomyTransaction StartingResources { get; }

        IEnumerable<IStartingUnit> StartingUnits { get; }

        IEnumerable<IStartingStructure> StartingStructures { get; }

        IEnumerable<IWaveTemplate> Waves { get; }
    }
}