﻿using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectile", menuName = "Abilities/Projectiles/Projectile")]
public class ProjectileTemplate : ScriptableObject {

    [SerializeField]
    protected AttackEffect[] attackEffects;

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected GameObject projectilePrefab;

    public AttackEffect[] AttackEffects
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

    public virtual Projectile GenerateProjectile(Vector3 startPosition, ITargetable target)
    {
        return new Projectile(startPosition, target, this);
    } 

}
