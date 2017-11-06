using GrimoireTD.Defenders.Structures;

namespace GrimoireTD.Economy
{
    public interface IStructureOccupationBonus : IOccupationBonus
    {
        IStructureTemplate StructureTemplate { get; }

        IStructureUpgrade StructureUpgradeLevel { get; }
    }
}