using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class SingleHexArgs : BuildModeAutoTargetedArgs
    {
        public SingleHexArgs(
            Coord targetCoord,
            IReadOnlyMapData mapData
        ) : base(targetCoord, mapData)
        { }
    }
}