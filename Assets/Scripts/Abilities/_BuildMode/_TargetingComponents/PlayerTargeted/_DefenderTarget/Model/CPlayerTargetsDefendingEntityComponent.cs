using System;
using System.Collections.Generic;
using GrimoireTD.Defenders;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class CPlayerTargetsDefenderComponent : IPlayerTargetsDefenderComponent
    {
        public IPlayerTargetsDefenderComponentTemplate PlayerTargetsDefenderComponentTemplate { get; }

        public CPlayerTargetsDefenderComponent(IPlayerTargetsDefenderComponentTemplate template)
        {
            PlayerTargetsDefenderComponentTemplate = template;
        }

        public bool IsValidTarget(
            IDefender sourceDefender, 
            IBuildModeTargetable potentialTarget
        )
        {
            IDefender potentialTargetDefender = potentialTarget as IDefender;

            if (potentialTargetDefender == null)
            {
                throw new ArgumentException("CPlayerTargetsDefenderComponent IsValidTarget was passed a non-Defender IBuildModeTargetable.");
            }

            return PlayerTargetsDefenderRuleService.RunRule(
                PlayerTargetsDefenderComponentTemplate.TargetingRule.GenerateArgs(
                    sourceDefender, 
                    potentialTargetDefender
                )
            );
        }

        public IReadOnlyList<IBuildModeTargetable> FindTargets(Coord position)
        {
            return BuildModeAutoTargetedRuleService.RunRule(
                PlayerTargetsDefenderComponentTemplate.AoeRule.GenerateArgs(position)
            );
        }
    }
}