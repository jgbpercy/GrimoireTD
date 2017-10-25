using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class SingleHexArgs : BuildModeAutoTargetedArgs
    {
        public SingleHexArgs(Coord targetCoord) : base(targetCoord) { }
    }
}