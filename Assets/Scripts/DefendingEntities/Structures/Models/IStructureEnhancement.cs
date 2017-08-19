using GrimoireTD.Economy;

namespace GrimoireTD.DefendingEntities.Structures
{
    public interface IStructureEnhancement
    {
        IEconomyTransaction Cost { get; }

        string DescriptionText { get; }

        IDefendingEntityImprovement EnhancementBonus { get; }
    }
}