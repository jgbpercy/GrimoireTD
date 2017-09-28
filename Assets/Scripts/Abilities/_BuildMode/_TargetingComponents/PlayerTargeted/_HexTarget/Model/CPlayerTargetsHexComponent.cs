using System;
using System.Collections.Generic;
using GrimoireTD.DefendingEntities;
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
            IDefendingEntity sourceDefendingEntity, 
            IBuildModeTargetable potentialTarget, 
            IReadOnlyMapData mapData
        )
        {
            Coord potentialTargetCoord = potentialTarget as Coord;

            if (potentialTargetCoord == null)
            {
                throw new ArgumentException("CPlayerTargetsHexComponent IsValidTarget was passed a non-coord IBuildModeTargetable");
            }

            return PlayerTargetsHexRuleService.RunRule(
                PlayerTargetsHexComponentTemplate.TargetingRule.GenerateArgs(
                    sourceDefendingEntity, 
                    potentialTargetCoord, 
                    mapData
                )
            );
        }

        public IReadOnlyList<IBuildModeTargetable> FindTargets(Coord position, IReadOnlyMapData mapData)
        {
            return BuildModeAbilityAutoTargetedRuleService.RunRule(
                PlayerTargetsHexComponentTemplate.AoeRule.GenerateArgs(position, mapData)
            );
        }
    }
}