﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using GrimoireTD.Creeps;
using GrimoireTD.DefendingEntities;
using GrimoireTD.DefendingEntities.Structures;
using GrimoireTD.DefendingEntities.Units;
using GrimoireTD.ChannelDebug;
using GrimoireTD.Technical;

namespace GrimoireTD.Abilities.DefendMode
{
    //TODO: refactor this to be a pure model class
    public class DefendModeAbilityController : MonoBehaviour
    {
        [SerializeField]
        private Component attachedToDefendingEntityComponent;

        private IDefendingEntity attachedToDefendingEntity;

        private IReadOnlyList<IDefendModeAbility> defendModeAbilities;

        private bool trackIdleTime = false;

        [SerializeField]
        private Transform firePoint;

        private void Start()
        {
            //TODO: refactor this crap - have a parent DE component (if there isn't one) with a reference the appropriate model
            //Yes I agree with my past self that this is horrible
            StructureComponent attachedToStructureComponent = attachedToDefendingEntityComponent as StructureComponent;
            UnitComponent attachedToUnitComponent = attachedToDefendingEntityComponent as UnitComponent;

            Assert.IsTrue(attachedToStructureComponent != null || attachedToUnitComponent != null);
            if (attachedToStructureComponent != null)
            {
                attachedToDefendingEntity = attachedToStructureComponent.StructureModel;
                trackIdleTime = false;
            }
            else
            {
                attachedToDefendingEntity = attachedToUnitComponent.UnitModel;
                trackIdleTime = true;
            }

            defendModeAbilities = attachedToDefendingEntity.Abilities.DefendModeAbilities();

            GameModels.Models[0].GameStateManager.OnEnterDefendMode += OnEnterDefendMode;
            //GameStateManager.Instance.RegisterForOnEnterBuildModeCallback(OnEnterBuildMode);
        }

        private void Update()
        {
            if (GameModels.Models[0].GameStateManager.CurrentGameMode != GameMode.DEFEND)
            {
                return;
            }

            ExecuteHighestPriorityAbilityOffCooldown();

            if (trackIdleTime)
            {
                if (defendModeAbilities.Any(x => !x.OffCooldown()))
                {
                    ((IUnit)attachedToDefendingEntity).TrackTime(false, Time.deltaTime);
                }
                else
                {
                    ((IUnit)attachedToDefendingEntity).TrackTime(true, Time.deltaTime);
                }
            }

        }

        private void ExecuteHighestPriorityAbilityOffCooldown()
        {
            for (int i = 0; i < defendModeAbilities.Count; i++)
            {
                if (defendModeAbilities[i].OffCooldown())
                {
                    if (defendModeAbilities[i].ExecuteAbility(attachedToDefendingEntity))
                    {
                        defendModeAbilities[i].WasExecuted();
                        CDebug.Log(CDebug.combatLog, attachedToDefendingEntity.Id + " (" + attachedToDefendingEntity.CurrentName + ") executed ability " + defendModeAbilities[i].DefendModeAbilityTemplate.NameInGame, this);
                        return;
                    }
                }
            }
        }

        private void OnEnterDefendMode(object sender, EAOnEnterDefendMode args)
        {
            defendModeAbilities = attachedToDefendingEntity.Abilities.DefendModeAbilities();
        }

        private void OnDestroy()
        {
            defendModeAbilities = attachedToDefendingEntity.Abilities.DefendModeAbilities();

            foreach (IDefendModeAbility defendModeAbility in defendModeAbilities)
            {
                ModelObjectFrameUpdater.Instance.DeregisterAsModelObjectFrameUpdatee(defendModeAbility);
            }
        }
    }
}