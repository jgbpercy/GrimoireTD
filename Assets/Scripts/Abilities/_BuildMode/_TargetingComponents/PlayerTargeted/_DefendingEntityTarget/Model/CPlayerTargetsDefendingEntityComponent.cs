﻿using System;
using System.Collections.Generic;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class CPlayerTargetsDefendingEntityComponent : IPlayerTargetsDefendingEntityComponent
    {
        public IPlayerTargetsDefendingEntityComponentTemplate PlayerTargetsDefendingEntityComponentTemplate { get; }

        public CPlayerTargetsDefendingEntityComponent(IPlayerTargetsDefendingEntityComponentTemplate template)
        {
            PlayerTargetsDefendingEntityComponentTemplate = template;
        }

        public bool IsValidTarget(
            IDefendingEntity sourceDefendingEntity, 
            IBuildModeTargetable potentialTarget, 
            IReadOnlyMapData mapData
        )
        {
            IDefendingEntity potentialTargetDefendingEntity = potentialTarget as IDefendingEntity;

            if (potentialTargetDefendingEntity == null)
            {
                throw new ArgumentException("CPlayerTargetsDefendingEntityComponent IsValidTarget was passed a non-DefendingEntity IBuildModeTargetable.");
            }

            return PlayerTargetsDefendingEntityRuleService.RunRule(
                PlayerTargetsDefendingEntityComponentTemplate.TargetingRule.GenerateArgs(
                    sourceDefendingEntity, 
                    potentialTargetDefendingEntity, 
                    mapData
                )
            );
        }

        public IReadOnlyList<IBuildModeTargetable> FindTargets(Coord position, IReadOnlyMapData mapData)
        {
            return BuildModeAutoTargetedRuleService.RunRule(
                PlayerTargetsDefendingEntityComponentTemplate.AoeRule.GenerateArgs(position, mapData)
            );
        }
    }
}