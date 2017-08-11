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

        IEnumerable<StartingUnit> StartingUnits { get; }

        IEnumerable<StartingStructure> StartingStructures { get; }

        IEnumerable<IWaveTemplate> Waves { get; }
    }
}