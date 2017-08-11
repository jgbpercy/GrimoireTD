using System.Collections.Generic;

namespace GrimoireTD.DefendingEntities.Units
{
    public interface IUnitTalent
    {
        IReadOnlyList<IUnitImprovement> UnitImprovements { get; }

        string DescriptionText { get; }
    }
}