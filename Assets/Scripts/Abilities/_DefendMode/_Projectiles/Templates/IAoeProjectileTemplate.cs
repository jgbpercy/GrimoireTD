using System.Collections.Generic;
using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    public interface IAoeProjectileTemplate : IProjectileTemplate
    {
        IEnumerable<IAttackEffect> AoeAttackEffects { get; }

        float AoeRadius { get; }

        float AoeExpansionLerpFactor { get; }
    }
}