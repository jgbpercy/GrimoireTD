using System.Collections.Generic;

namespace GrimoireTD.Defenders.Units
{
    public interface IUnitTalent
    {
        IReadOnlyList<IUnitImprovement> UnitImprovements { get; }

        string DescriptionText { get; }
    }
}