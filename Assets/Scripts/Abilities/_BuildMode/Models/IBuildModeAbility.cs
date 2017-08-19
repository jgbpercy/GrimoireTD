using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IBuildModeAbility : IAbility
    {
        IBuildModeAbilityTemplate BuildModeAbilityTemplate { get; }

        void ExecuteAbility(IDefendingEntity executingEntity, Coord executionPosition);
    }
}