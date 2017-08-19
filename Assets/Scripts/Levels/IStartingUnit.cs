using GrimoireTD.DefendingEntities.Units;
using GrimoireTD.Map;

namespace GrimoireTD.Levels
{
    public interface IStartingUnit
    {
        IUnitTemplate UnitTemplate { get; }

        Coord StartingPosition { get; }
    }
}