using System.Collections.Generic;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class CAutoTargetedComponent : IAutoTargetedComponent
    {
        public IAutoTargetedComponentTemplate AutoTargetedComponentTemplate { get; }

        public CAutoTargetedComponent(IAutoTargetedComponentTemplate template)
        {
            AutoTargetedComponentTemplate = template;
        }

        public IReadOnlyList<IBuildModeTargetable> FindTargets(
            Coord position, 
            IReadOnlyMapData mapData
        )
        {
            return BuildModeAutoTargetedRuleService.RunRule(
                AutoTargetedComponentTemplate.TargetingRule.GenerateArgs(position, mapData)
            );
        }
    }
}