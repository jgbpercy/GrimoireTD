public interface IAbilityTemplate {

    string NameInGame { get; }

    Ability GenerateAbility(DefendingEntity attachedToDefendingEntity);
}
