using System.Collections.Generic;
using GrimoireTD.Economy;
using GrimoireTD.Map;

namespace GrimoireTD.Defenders.Structures
{
    public interface IStructureTemplate : IDefenderTemplate
    {
        string StartingNameInGame { get; }

        string StartingDescription { get; }

        IEconomyTransaction Cost { get; }

        IEnumerable<IStructureUpgrade> StructureUpgrades { get; }

        string UIText();

        IStructure GenerateStructure(Coord position);
    }
}