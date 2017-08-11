using System.Collections.Generic;
using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    public interface IAoeProjectileTemplate : IProjectileTemplate
    {
        IEnumerable<AttackEffect> AoeAttackEffects { get; }

        float AoeRadius { get; }

        float AoeExpansionLerpFactor { get; }
    }
}