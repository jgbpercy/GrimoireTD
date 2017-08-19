using GrimoireTD.Attributes;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public interface IAttributeModifierEffectType
    {
        CreepAttributeName CreepAttributeName { get; }
    }
}