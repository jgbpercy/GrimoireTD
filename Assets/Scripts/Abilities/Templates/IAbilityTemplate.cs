using GrimoireTD.Defenders;

namespace GrimoireTD.Abilities
{
    public interface IAbilityTemplate
    {
        string NameInGame { get; }

        IAbility GenerateAbility(IDefender attachedToDefender);
    }
}