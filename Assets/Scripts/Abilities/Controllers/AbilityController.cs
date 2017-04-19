using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AbilityController : MonoBehaviour {

    [SerializeField]
    private Component attachedToDefendingEntityComponent;

    private DefendingEntity attachedToDefendingEntity;

    private List<DefendModeAbility> defendModeAbilities;

    [SerializeField]
    private Transform firePoint;

    void Start () {

        StructureComponent attachedToStructureComponent = attachedToDefendingEntityComponent as StructureComponent;
        UnitComponent attachedToUnitComponent = attachedToDefendingEntityComponent as UnitComponent;

        Assert.IsTrue(attachedToStructureComponent != null || attachedToUnitComponent != null);
        if (attachedToStructureComponent != null )
        {
            attachedToDefendingEntity = attachedToStructureComponent.StructureModel;
        }
        else
        {
            attachedToDefendingEntity = attachedToUnitComponent.UnitModel;
        }

        defendModeAbilities = attachedToDefendingEntity.DefendModeAbilities();

        GameStateManager.Instance.RegisterForOnEnterDefendModeCallback(() => OnEnterDefendMode());
    }

    private void Update ()
    {
        ExecuteHighestPriorityAbilityOffCooldown();
	}

    private void ExecuteHighestPriorityAbilityOffCooldown()
    {
        for (int i = 0; i < defendModeAbilities.Count; i++)
        {
            if (defendModeAbilities[i].OffCooldown())
            {
                if ( defendModeAbilities[i].ExecuteAbility(firePoint.position) )
                {
                    defendModeAbilities[i].WasExecuted();
                    return;
                }
            }
        }
    }

    private void OnEnterDefendMode()
    {
        defendModeAbilities = attachedToDefendingEntity.DefendModeAbilities();
    }

    private void OnDestroy()
    {
        defendModeAbilities = attachedToDefendingEntity.DefendModeAbilities();

        foreach(DefendModeAbility defendModeAbility in defendModeAbilities)
        {
            ModelObjectFrameUpdater.Instance.DeregisterAsModelObjectFrameUpdatee(defendModeAbility);
        }
    }

}
