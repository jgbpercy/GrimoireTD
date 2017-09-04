using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IBuildModeAutoTargetedArgsTemplate
    {
        BuildModeAutoTargetedArgs GenerateArgs(Coord targetCoord, IReadOnlyMapData mapData);
    }
}