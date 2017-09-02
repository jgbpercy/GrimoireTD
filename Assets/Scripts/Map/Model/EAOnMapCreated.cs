using System;

namespace GrimoireTD.Map
{
    public class EAOnMapCreated : EventArgs
    {
        public readonly IReadOnlyMapData MapData;

        public EAOnMapCreated(IReadOnlyMapData mapData)
        {
            MapData = mapData;
        }
    }
}