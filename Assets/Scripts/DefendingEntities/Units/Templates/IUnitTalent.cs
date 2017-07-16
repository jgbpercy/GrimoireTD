using System.Collections.Generic;

public interface IUnitTalent {

    IList<IUnitImprovement> UnitImprovements { get; }

    string DescriptionText { get; }
}