using System;
using System.Collections.Generic;
using GrimoireTD.Defenders;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class CPlayerTargetsHexComponent : IPlayerTargetsHexComponent
    {
        public IPlayerTargetsHexComponentTemplate PlayerTargetsHexComponentTemplate { get; }

        public CPlayerTargetsHexComponent(IPlayerTargetsHexComponentTemplate template)
        {
            PlayerTargetsHexComponentTemplate = template;
        }

        public bool IsValidTarget(
            IDefender sourceDefender, 
            IBuildModeTargetable potentialTarget        
        )
        {
            Coord potentialTargetCoord = potentialTarget as Coord;

            if (potentialTargetCoord == null)
            {
                throw new ArgumentException("CPlayerTargetsHexComponent IsValidTarget was passed a non-coord IBuildModeTargetable");
            }

            return PlayerTargetsHexRuleService.RunRule(
                PlayerTargetsHexComponentTemplate.TargetingRule.GenerateArgs(
                    sourceDefender, 
                    potentialTargetCoord
                )
            );
        }

        public IReadOnlyList<IBuildModeTargetable> FindTargets(Coord position)
        {
            return BuildModeAutoTargetedRuleService.RunRule(
                PlayerTargetsHexComponentTemplate.AoeRule.GenerateArgs(position)
            );
        }
    }
}