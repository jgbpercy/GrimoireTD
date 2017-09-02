namespace GrimoireTD.Abilities
{
    public interface IAbilities : IReadOnlyAbilities
    {
        void AddAbility(IAbility ability);

        bool TryRemoveAbility(IAbility ability);
        bool TryRemoveAbility(IAbilityTemplate template);
    }
}