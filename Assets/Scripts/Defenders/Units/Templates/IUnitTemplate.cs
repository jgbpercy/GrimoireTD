using System.Collections.Generic;
using GrimoireTD.Map;

namespace GrimoireTD.Defenders.Units
{
    public interface IUnitTemplate : IDefenderTemplate
    {
        string NameInGame { get; }

        string Description { get; }

        int ExperienceToLevelUp { get; }

        IEnumerable<IUnitTalent> UnitTalents { get; }

        IUnit GenerateUnit(Coord position);
    }
}