using System.Collections.Generic;
using UnityEngine.Assertions;
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

            Assert.IsTrue(potentialTargetDefendingEntity != null);

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
            return BuildModeAbilityAutoTargetedRuleService.RunRule(
                PlayerTargetsDefendingEntityComponentTemplate.AoeRule.GenerateArgs(position, mapData)
            );
        }
    }
}