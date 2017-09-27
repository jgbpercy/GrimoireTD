using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class BuildModeAutoTargetedArgs
    {
        public readonly Coord TargetCoord;

        public readonly IReadOnlyMapData MapData;

        public BuildModeAutoTargetedArgs(
            Coord targetCoord,
            IReadOnlyMapData mapData
        )
        {
            TargetCoord = targetCoord;
            MapData = mapData;
        }
    }
}