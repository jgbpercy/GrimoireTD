using System;
using System.Collections.Generic;
using GrimoireTD.Defenders;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class CBuildModeAbility : CAbility, IBuildModeAbility
    {
        public IBuildModeAbilityTemplate BuildModeAbilityTemplate { get; }

        public IBuildModeTargetingComponent TargetingComponent { get; }

        private List<IBuildModeEffectComponent> effectComponents;

        public event EventHandler<EAOnExecutedBuildModeAbility> OnExecuted;

        public CBuildModeAbility(IBuildModeAbilityTemplate template) : base(template)
        {
            BuildModeAbilityTemplate = template;

            TargetingComponent = template.TargetingComponentTemplate.GenerateTargetingComponent();

            effectComponents = new List<IBuildModeEffectComponent>();

            foreach (var effectComponentTemplate in template.EffectComponentTemplates)
            {
                effectComponents.Add(effectComponentTemplate.GenerateEffectComponent());
            }
        }

        public void ExecuteAbility(IDefender executingDefender, Coord executionPosition)
        {
            IReadOnlyList<IBuildModeTargetable> targetList = TargetingComponent.FindTargets(executionPosition);

            foreach(var effectComponent in effectComponents)
            {
                effectComponent.ExecuteEffect(executingDefender, targetList);
            }

            OnExecuted?.Invoke(this, new EAOnExecutedBuildModeAbility(this));
        }

        public override string UIText()
        {
            return BuildModeAbilityTemplate.NameInGame;
        }
    }
}