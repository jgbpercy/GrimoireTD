using System.Collections.Generic;
using UnityEngine;

public interface IStructureTemplate : IDefendingEntityTemplate {

    string StartingNameInGame { get; }

    string StartingDescription { get; }

    IEconomyTransaction Cost { get; }

    IEnumerable<IStructureUpgrade> StructureUpgrades { get; }

    string UIText();

    Structure GenerateStructure(Coord position);
}
