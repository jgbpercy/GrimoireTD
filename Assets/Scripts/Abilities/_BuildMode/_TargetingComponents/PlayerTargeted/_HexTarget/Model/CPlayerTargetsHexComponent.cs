using System.Collections.Generic;
using UnityEngine.Assertions;
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

            Assert.IsTrue(potentialTargetCoord != null);

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