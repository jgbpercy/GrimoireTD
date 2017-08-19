using GrimoireTD.DefendingEntities.Structures;
using GrimoireTD.Map;

namespace GrimoireTD.Levels
{
    public interface IStartingStructure
    {
        IStructureTemplate StructureTemplate { get; }

        Coord StartingPosition { get; }
    }
}