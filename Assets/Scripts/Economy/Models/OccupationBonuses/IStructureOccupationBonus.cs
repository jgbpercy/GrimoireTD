using GrimoireTD.DefendingEntities.Structures;

namespace GrimoireTD.Economy
{
    public interface IStructureOccupationBonus : IOccupationBonus
    {
        IStructureTemplate StructureTemplate { get; }

        IStructureUpgrade StructureUpgradeLevel { get; }
    }
}