﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class AbilityController : MonoBehaviour {

    [SerializeField]
    private Component attachedToDefendingEntityComponent;

    private DefendingEntity attachedToDefendingEntity;

    private IReadOnlyList<DefendModeAbility> defendModeAbilities;

    private bool trackIdleTime = false;

    [SerializeField]
    private Transform firePoint;

    private void Start ()
    {
        StructureComponent attachedToStructureComponent = attachedToDefendingEntityComponent as StructureComponent;
        UnitComponent attachedToUnitComponent = attachedToDefendingEntityComponent as UnitComponent;

        Assert.IsTrue(attachedToStructureComponent != null || attachedToUnitComponent != null);
        if (attachedToStructureComponent != null )
        {
            attachedToDefendingEntity = attachedToStructureComponent.StructureModel;
            trackIdleTime = false;
        }
        else
        {
            attachedToDefendingEntity = attachedToUnitComponent.UnitModel;
            trackIdleTime = true;
        }

        defendModeAbilities = attachedToDefendingEntity.DefendModeAbilities();

        GameStateManager.Instance.RegisterForOnEnterDefendModeCallback(OnEnterDefendMode);
        //GameStateManager.Instance.RegisterForOnEnterBuildModeCallback(OnEnterBuildMode);
    }

    private void Update ()
    {
        if (GameStateManager.Instance.CurrentGameMode != GameMode.DEFEND)
        {
            return;
        }

        ExecuteHighestPriorityAbilityOffCooldown();

        if ( trackIdleTime && CreepManager.Instance.TrackIdleTime )
        {
            if ( defendModeAbilities.Any(x => !x.OffCooldown() ))
            {
                ((Unit)attachedToDefendingEntity).TrackTime(false, Time.deltaTime);
            }
            else
            {
                ((Unit)attachedToDefendingEntity).TrackTime(true, Time.deltaTime);
            }
        }
            
    }

    private void ExecuteHighestPriorityAbilityOffCooldown()
    {
        for (int i = 0; i < defendModeAbilities.Count; i++)
        {
            if (defendModeAbilities[i].OffCooldown())
            {
                if ( defendModeAbilities[i].ExecuteAbility(attachedToDefendingEntity) )
                {
                    defendModeAbilities[i].WasExecuted();
                    CDebug.Log(CDebug.combatLog, attachedToDefendingEntity.Id + " (" + attachedToDefendingEntity.CurrentName() + ") executed ability " + defendModeAbilities[i].DefendModeAbilityTemplate.NameInGame, this);
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
