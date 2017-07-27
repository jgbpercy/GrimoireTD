using System.Collections.Generic;

public interface IUnitTalent {

    IReadOnlyList<IUnitImprovement> UnitImprovements { get; }

    string DescriptionText { get; }
}