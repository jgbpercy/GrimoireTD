using System.Collections.Generic;
using UnityEngine;

public interface IProjectileTemplate {

    IEnumerable<AttackEffect> AttackEffects { get; }

    float Speed { get; }

    GameObject ProjectilePrefab { get; }

    Projectile GenerateProjectile(Vector3 startPosition, IDefendModeTargetable target, DefendingEntity sourceDefendingEntity);
}
