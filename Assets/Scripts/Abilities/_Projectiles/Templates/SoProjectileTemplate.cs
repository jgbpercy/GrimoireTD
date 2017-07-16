using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewProjectile", menuName = "Defend Mode Abilities/Projectiles/Projectile")]
public class SoProjectileTemplate : ScriptableObject, IProjectileTemplate {

    [SerializeField]
    protected AttackEffect[] attackEffects;

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected GameObject projectilePrefab;

    public IEnumerable<AttackEffect> AttackEffects
    {
        get
        {
            return attackEffects;
        }
    }

    public float Speed
    {
        get
        {
            return speed; 
        }
    }

    public GameObject ProjectilePrefab
    {
        get
        {
            return projectilePrefab;
        }
    }

    public virtual Projectile GenerateProjectile(Vector3 startPosition, IDefendModeTargetable target, DefendingEntity sourceDefendingEntity)
    {
        return new Projectile(startPosition, target, this, sourceDefendingEntity);
    } 

}
