using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class DefendModeAbility : Ability, IFrameUpdatee {

    private float timeSinceExecuted;

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

    public DefendModeAbilityTemplate DefendModeAbilityTemplate
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

    public override string UIText()
    {
        return defendModeAbilityTemplate.NameInGame;
    }

    public bool ExecuteAbility(Vector3 executionPosition)
    {
        List<ITargetable> targetList = defendModeAbilityTemplate.TargetingComponent.FindTargets(executionPosition);

        if ( targetList == null ) { return false; }

        defendModeAbilityTemplate.EffectComponent.ExecuteEffect(executionPosition, targetList);
        return true;
    }

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
