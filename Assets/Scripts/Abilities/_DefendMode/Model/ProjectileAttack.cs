using System;
using UnityEngine;

public class ProjectileAttack : DefendModeAbility
{
    private ProjectileAttackTemplate projectileAttackTemplate;

    public ProjectileAttackTemplate ProjectileAttackTemplate
    {
        get
        {
            return projectileAttackTemplate;
        }
    }

    public ProjectileAttack(ProjectileAttackTemplate template) : base(template)
    {
        findTarget = CreepManager.CreepInRangeNearestToEnd;

        projectileAttackTemplate = template;
    }

    public override bool ExecuteAbility(Vector3 executionPosition)
    {
        ITargetable potentialTarget = findTarget(executionPosition, projectileAttackTemplate.Range);

        if (potentialTarget == null)
        {
            return false;
        }

        projectileAttackTemplate.ProjectileToFireTemplate.GenerateProjectile(executionPosition, potentialTarget);

        return true;
    }

    public override string UIText()
    {
        throw new NotImplementedException();
    }
}
