using GrimoireTD.Economy;

namespace GrimoireTD.Defenders.Structures
{
    public interface IStructureEnhancement
    {
        IEconomyTransaction Cost { get; }

        string DescriptionText { get; }

        IDefenderImprovement EnhancementBonus { get; }
    }
}