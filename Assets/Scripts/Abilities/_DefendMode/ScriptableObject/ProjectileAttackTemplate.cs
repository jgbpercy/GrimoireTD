using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectileAttack", menuName = "Abilities/Projectile Attack")]
public class ProjectileAttackTemplate : DefendModeAbilityTemplate {

    [SerializeField]
    protected float range;

    [SerializeField]
    protected ProjectileTemplate projectileToFireTemplate;

    public float Range
    {
        get
        {
            return range;
        }
    }

    public ProjectileTemplate ProjectileToFireTemplate
    {
        get
        {
            return projectileToFireTemplate;
        }
    }

    public override Ability GenerateAbility()
    {
        return new ProjectileAttack(this);
    }
}
