using System.Collections.Generic;
using UnityEngine;

public class DefendModeAbility : Ability, IFrameUpdatee {

    private int id;

    private float timeSinceExecuted;
    private float actualCooldown;

    private IDefendModeAbilityTemplate defendModeAbilityTemplate;

    private DefendingEntity attachedToDefendingEntity;

    public float TimeSinceExecuted
    {
        get
        {
            return timeSinceExecuted;
        }
    }

    public float TimeSinceExecutedClamped
    {
        get
        {
            return Mathf.Clamp(timeSinceExecuted, 0f, actualCooldown);
        }
    }

    public float ActualCooldown
    {
        get
        {
            return actualCooldown;
        }
    }

    public float PercentOfCooldownPassed
    {
        get
        {
            return Mathf.Clamp(timeSinceExecuted / actualCooldown, 0f, 1f);
        }
    }

    public string Id
    {
        get
        {
            return "DefAb-" + id;
        }
    }

    public IDefendModeAbilityTemplate DefendModeAbilityTemplate
    {
        get
        {
            return defendModeAbilityTemplate;
        }
    }

    public DefendModeAbility(IDefendModeAbilityTemplate template, DefendingEntity attachedToDefendingEntity) : base(template, attachedToDefendingEntity)
    {
        id = IdGen.GetNextId();

        timeSinceExecuted = 0f;
        defendModeAbilityTemplate = template;

        this.attachedToDefendingEntity = attachedToDefendingEntity;

        actualCooldown = GetActualCooldown();

        attachedToDefendingEntity.RegisterForOnAttributeChangedCallback(OnAttachedDefendingEntityCooldownReductionChange, AttributeName.cooldownReduction);

        ModelObjectFrameUpdater.Instance.RegisterAsModelObjectFrameUpdatee(this);
    }

    public override string UIText()
    {
        return defendModeAbilityTemplate.NameInGame;
    }

    public bool ExecuteAbility(DefendingEntity attachedToDefendingEntity)
    {
        IReadOnlyList<IDefendModeTargetable> targetList = defendModeAbilityTemplate.TargetingComponent.FindTargets(attachedToDefendingEntity);

        if ( targetList == null ) { return false; }

        defendModeAbilityTemplate.EffectComponent.ExecuteEffect(attachedToDefendingEntity, targetList);
        return true;
    }

    private float GetActualCooldown()
    {
        float cooldown = GetCooldownAfterReduction(attachedToDefendingEntity.GetAttribute(AttributeName.cooldownReduction));

        CDebug.Log(CDebug.unitAttributes, Id + " (" + defendModeAbilityTemplate.NameInGame + ") cooldown: " + cooldown + " = " + defendModeAbilityTemplate.BaseCooldown + " * " + (1 - attachedToDefendingEntity.GetAttribute(AttributeName.cooldownReduction)));

        return cooldown;
    }

    private float GetCooldownAfterReduction(float cooldownReduction)
    {
        return defendModeAbilityTemplate.BaseCooldown * (1 - cooldownReduction);
    }

    private void OnAttachedDefendingEntityCooldownReductionChange(float newCooldownReduction)
    {
        float newCooldown = GetCooldownAfterReduction(newCooldownReduction);

        timeSinceExecuted = newCooldown * PercentOfCooldownPassed;

        actualCooldown = newCooldown;
    }

    public bool OffCooldown()
    {
        return timeSinceExecuted > actualCooldown;
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
