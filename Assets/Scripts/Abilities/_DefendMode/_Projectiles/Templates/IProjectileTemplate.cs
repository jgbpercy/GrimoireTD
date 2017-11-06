using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.Defenders;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    public interface IProjectileTemplate
    {
        IEnumerable<IAttackEffect> AttackEffects { get; }

        float Speed { get; }

        IProjectile GenerateProjectile(Vector3 startPosition, IDefendModeTargetable target, IDefender sourceDefender);
    }
}