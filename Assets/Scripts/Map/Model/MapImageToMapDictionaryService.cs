using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GrimoireTD.Map
{
    public static class MapImageToMapDictionaryService
    {
        public static Func<
            Texture2D,
            IDictionary<Color32, IHexType>,
            Dictionary<Coord, IHexData>>
        GetMapDictionary =
            (mapImage,
            colorsToTypesDictionary) =>
        {
            var hexes = new Dictionary<Coord, IHexData>();

            var allPixels = mapImage.GetPixels32();
            int xPixelCoord;
            int yPixelCoord;

            IHexType currentHexType;
            bool foundHexType;

            for (var x = 0; x < mapImage.width / 2; x++)
            {
                for (var y = 0; y < mapImage.height / 2; y++)
                {
                    xPixelCoord = y % 2 == 0 ? 2 * x : 2 * x + 1;
                    yPixelCoord = 2 * y;

                    foundHexType = colorsToTypesDictionary.TryGetValue(allPixels[xPixelCoord + yPixelCoord * mapImage.width], out currentHexType);

                    Assert.IsTrue(foundHexType);

                    hexes.Add(new Coord(x, y), new CHexData(currentHexType));
                }
            }

            return hexes;
        };
    }
}