namespace GrimoireTD.Abilities
{
    public interface IAbility
    {
        IAbilityTemplate Template { get; }

        string UIText();
    }
}