using System.Collections.Generic;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public interface IReadOnlyAttackEffectTypeManager
    {
        IReadOnlyList<IMetaDamageEffectType> MetaDamageTypes { get; }
        IReadOnlyList<IBasicMetaDamageEffectType> BasicMetaDamageTypes { get; }
        IReadOnlyList<IStrongMetaDamageEffectType> StrongMetaDamageTypes { get; }
        IReadOnlyList<IWeakMetaDamageEffectType> WeakMetaDamageTypes { get; }
        IReadOnlyList<ISpecificDamageEffectType> SpecificDamageTypes { get; }
        IReadOnlyList<IModifierEffectType> ModifierEffectTypes { get; }
        IReadOnlyList<IAttributeModifierEffectType> AttributeEffectTypes { get; }
        IReadOnlyList<IResistanceModifierEffectType> ResistanceEffectTypes { get; }

        IBasicMetaDamageEffectType GetBasicMetaDamageType(ISpecificDamageEffectType specificDamageType);
        IStrongMetaDamageEffectType GetStrongMetaDamageType(IBasicMetaDamageEffectType basicDamageType);
        IStrongMetaDamageEffectType GetStrongMetaDamageType(ISpecificDamageEffectType specificDamageType);
        IWeakMetaDamageEffectType GetWeakMetaDamageType(IBasicMetaDamageEffectType basicDamageType);
        IWeakMetaDamageEffectType GetWeakMetaDamageType(ISpecificDamageEffectType specificDamageType);
        IReadOnlyList<ISpecificDamageEffectType> GetSpecificDamageTypes(IMetaDamageEffectType metaDamageEffectType);
    }
}