using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class BuildModeAutoTargetedArgs
    {
        public readonly Coord TargetCoord;

        public BuildModeAutoTargetedArgs(Coord targetCoord)
        {
            TargetCoord = targetCoord;
        }
    }
}