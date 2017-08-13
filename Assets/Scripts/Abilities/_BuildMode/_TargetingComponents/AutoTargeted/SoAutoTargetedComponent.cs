﻿using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class SoAutoTargetedComponent : SoBuildModeTargetingComponent, IAutoTargetedComponent
    {
        [SerializeField]
        private BuildModeAbilityAutoTargetedRuleService.RuleName targetingRule;

        public override IReadOnlyList<IBuildModeTargetable> FindTargets(Coord position)
        {
            return BuildModeAbilityAutoTargetedRuleService.RunRule(targetingRule, position);
        }
    }
}