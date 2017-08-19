using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities
{
    public interface IAbilityTemplate
    {
        string NameInGame { get; }

        IAbility GenerateAbility(IDefendingEntity attachedToDefendingEntity);
    }
}