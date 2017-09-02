namespace GrimoireTD.Abilities
{
    public interface IAbility
    {
        IAbilityTemplate AbilityTemplate { get; }

        string UIText();
    }
}