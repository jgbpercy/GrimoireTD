using System.Collections.Generic;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class CBuildModeAbility : CAbility, IBuildModeAbility
    {
        private IBuildModeAbilityTemplate buildModeAbilityTemplate;

        public IBuildModeAbilityTemplate BuildModeAbilityTemplate
        {
            get
            {
                return buildModeAbilityTemplate;
            }
        }

        public CBuildModeAbility(IBuildModeAbilityTemplate template) : base(template)
        {
            buildModeAbilityTemplate = template;
        }

        public void ExecuteAbility(IDefendingEntity executingEntity, Coord executionPosition)
        {
            IReadOnlyList<IBuildModeTargetable> targetList = buildModeAbilityTemplate.TargetingComponent.FindTargets(executionPosition);

            buildModeAbilityTemplate.EffectComponent.ExecuteEffect(executingEntity, targetList);

            buildModeAbilityTemplate.Cost.DoTransaction();
        }

        public override string UIText()
        {
            return buildModeAbilityTemplate.NameInGame;
        }
    }
}