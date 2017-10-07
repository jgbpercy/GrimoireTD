using GrimoireTD.Attributes;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public interface IAttributeModifierEffectType : IModifierEffectType
    {
        CreepAttrName CreepAttributeName { get; }
    }
}