using System;
using UnityEngine;

public abstract class DefendModeAbility : Ability, IFrameUpdatee {

    protected float timeSinceExecuted;

    private DefendModeAbilityTemplate defendModeAbilityTemplate;

    public float TimeSinceExecuted
    {
        get
        {
            return timeSinceExecuted;
        }
    }

    public float PercentOfCooldownPassed
    {
        get
        {
            return Mathf.Clamp(timeSinceExecuted / defendModeAbilityTemplate.Cooldown, 0f, 1f);
        }
    }

    public DefendModeAbilityTemplate DefendModeAbilityClassTemplate
    {
        get
        {
            return defendModeAbilityTemplate;
        }
    }

    public DefendModeAbility(DefendModeAbilityTemplate template) : base(template)
    {
        timeSinceExecuted = 0f;
        defendModeAbilityTemplate = template;

        ModelObjectFrameUpdater.Instance.RegisterAsModelObjectFrameUpdatee(this);
    }

    public abstract bool ExecuteAbility(Vector3 executionPosition);

    public bool OffCooldown()
    {
        return timeSinceExecuted > defendModeAbilityTemplate.Cooldown;
    }

    public void ModelObjectFrameUpdate()
    {
        timeSinceExecuted += Time.deltaTime;
    }

    public void WasExecuted()
    {
        timeSinceExecuted = 0f;
    }

    public void GameObjectDestroyed()
    {
        ModelObjectFrameUpdater.Instance.DeregisterAsModelObjectFrameUpdatee(this);
    }
}
