using System.Collections.Generic;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public interface IAttackEffectTypeManager : IReadOnlyAttackEffectTypeManager
    {
        void SetUp(IEnumerable<IAttackEffectType> attackEffectTypes);
    }
}