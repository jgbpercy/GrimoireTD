using System;
using System.Collections.Generic;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class CBuildModeAbility : CAbility, IBuildModeAbility
    {
        public IBuildModeAbilityTemplate BuildModeAbilityTemplate { get; }

        private IBuildModeTargetingComponent targetingComponent;

        private List<IBuildModeEffectComponent> effectComponents;

        public event EventHandler<EAOnExecutedBuildModeAbility> OnExecuted;

        public CBuildModeAbility(IBuildModeAbilityTemplate template) : base(template)
        {
            BuildModeAbilityTemplate = template;

            targetingComponent = template.TargetingComponent;

            effectComponents = new List<IBuildModeEffectComponent>();

            foreach (var effectComponentTemplate in template.EffectComponents)
            {
                effectComponents.Add(effectComponentTemplate.GenerateEffectComponent());
            }
        }

        public void ExecuteAbility(IDefendingEntity executingEntity, Coord executionPosition, IReadOnlyMapData mapData)
        {
            IReadOnlyList<IBuildModeTargetable> targetList = BuildModeAbilityTemplate.TargetingComponent.FindTargets(executionPosition, mapData);

            foreach(var effectComponent in effectComponents)
            {
                effectComponent.ExecuteEffect(executingEntity, targetList);
            }

            OnExecuted?.Invoke(this, new EAOnExecutedBuildModeAbility(this));
        }

        public override string UIText()
        {
            return BuildModeAbilityTemplate.NameInGame;
        }
    }
}