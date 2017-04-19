using UnityEngine;
using System;

public class Projectile : IFrameUpdatee {

    //TODO check objects like this are destroyed and garbage collected somehow :)
    private Vector3 position;
    private Vector3 currentDirection;

    private ITargetable target;

    protected bool destroyingForHitTarget;
    private bool destroyingForNoTarget;

    private ProjectileTemplate projectileTemplate;

    private Action<float> OnDestroyCallback;

    public Vector3 Position
    {
        get
        {
            return position;
        }
    }

    public ProjectileTemplate ProjectileClassTemplate
    {
        get
        {
            return projectileTemplate;
        }
    }

    public Projectile(Vector3 startPosition, ITargetable target, ProjectileTemplate template)
    {
        position = startPosition;
        this.target = target;
        projectileTemplate = template;
        ProjectileView.Instance.CreateProjectile(this);

        ModelObjectFrameUpdater.Instance.RegisterAsModelObjectFrameUpdatee(this);
    }

    public virtual void ModelObjectFrameUpdate()
    {
        if (destroyingForHitTarget)
        {
            return;
        }

        if (target == null)
        {
            if (!destroyingForNoTarget)
            {
                Destroy(5f);
                destroyingForNoTarget = true;
            }

            if (currentDirection == Vector3.zero)
            {
                Destroy();
            }
            position = position + currentDirection * projectileTemplate.Speed * Time.deltaTime;
        }
        else
        {
            position = Vector3.MoveTowards(position, target.TargetPosition(), projectileTemplate.Speed * Time.deltaTime);
            currentDirection = (target.TargetPosition() - position).normalized;
        }
    }

    public void HitCreep(Creep creep, float destructionDelay)
    {
        creep.TakeDamage(projectileTemplate.Damage);
        destroyingForHitTarget = true;
        Destroy(destructionDelay);
    }

    private void Destroy()
    {
        OnDestroyCallback(0f);
    }

    private void Destroy(float waitSeconds)
    {
        OnDestroyCallback(waitSeconds);
    }

    public void GameObjectDestroyed()
    {
        ModelObjectFrameUpdater.Instance.DeregisterAsModelObjectFrameUpdatee(this);
    }

    public void RegisterForOnDestroyCallback(Action<float> callback)
    {
        OnDestroyCallback += callback;
    }

    public void DeregisterForOnDestroyCallback(Action<float> callback)
    {
        OnDestroyCallback -= callback;
    }

}
