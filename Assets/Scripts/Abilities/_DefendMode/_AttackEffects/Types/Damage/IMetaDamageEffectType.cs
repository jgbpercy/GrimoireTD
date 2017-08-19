using System.Collections.Generic;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public interface IMetaDamageEffectType : IDamageEffectType
    {
        IReadOnlyList<ISpecificDamageEffectType> SpecificDamageTypes { get; }
    }
}