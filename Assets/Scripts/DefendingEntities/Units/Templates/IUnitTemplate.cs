using System.Collections.Generic;

public interface IUnitTemplate: IDefendingEntityTemplate {

    string NameInGame { get; }

    string Description { get; }

    int ExperienceToLevelUp { get; }

    IEnumerable<IUnitTalent> UnitTalents { get; }

    IUnitImprovement BaseUnitCharacteristics { get; }

    Unit GenerateUnit(Coord position);
}
