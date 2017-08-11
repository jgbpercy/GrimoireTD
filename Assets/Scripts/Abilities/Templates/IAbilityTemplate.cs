using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities
{
    public interface IAbilityTemplate
    {
        string NameInGame { get; }

        Ability GenerateAbility(DefendingEntity attachedToDefendingEntity);
    }
}