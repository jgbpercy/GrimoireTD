using System.Collections.Generic;
using GrimoireTD.Map;

namespace GrimoireTD.DefendingEntities.Units
{
    public interface IUnitTemplate : IDefendingEntityTemplate
    {
        string NameInGame { get; }

        string Description { get; }

        int ExperienceToLevelUp { get; }

        IEnumerable<IUnitTalent> UnitTalents { get; }

        IUnit GenerateUnit(Coord position);
    }
}