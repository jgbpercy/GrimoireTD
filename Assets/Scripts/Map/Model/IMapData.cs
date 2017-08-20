using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Levels;

namespace GrimoireTD.Map
{
    public interface IMapData : IReadOnlyMapData
    {
        void SetUp(
            Texture2D mapImage,
            IDictionary<Color32, IHexType> colorsToTypesDictionary,
            IEnumerable<IStartingStructure> startingStructures,
            IEnumerable<IStartingUnit> startingUnits
        );
    }
}