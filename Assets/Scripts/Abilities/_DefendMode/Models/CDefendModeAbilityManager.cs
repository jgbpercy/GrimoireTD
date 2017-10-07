using System;
using System.Collections.Generic;
using GrimoireTD.DefendingEntities;
using GrimoireTD.ChannelDebug;
using GrimoireTD.Technical;

namespace GrimoireTD.Abilities.DefendMode
{
    public class CDefendModeAbilityManager : IDefendModeAbilityManager
    {
        private IDefendingEntity attachedToDefendingEntity;

        private List<IDefendModeAbility> offCooldownAbilities = new List<IDefendModeAbility>();
        private int numberOfAbilitiesOnCooldown;

        public event EventHandler<EAOnAllDefendModeAbilitiesOffCooldown> OnAllDefendModeAbilitiesOffCooldown;

        public CDefendModeAbilityManager(IAbilities abilities, IDefendingEntity attachedToDefendingEntity)
        {
            numberOfAbilitiesOnCooldown = 0;

            foreach (var ability in abilities.DefendModeAbilities())
            {
                OnNewAbility(ability);
            }

            abilities.OnDefendModeAbilityAdded += OnDefendModeAbilityAdded;
            abilities.OnDefendModeAbilityRemoved += OnDefendModeAbilityRemoved;

            this.attachedToDefendingEntity = attachedToDefendingEntity;

            ModelObjectFrameUpdater.Instance.RegisterAsModelObjectFrameUpdatee(this);
        }

        public void ModelObjectFrameUpdate(float deltaTime)
        {
            if (GameModels.Models[0].GameStateManager.CurrentGameMode != GameMode.DEFEND)
            {
                return;
            }

            foreach (var abilityOffCooldown in offCooldownAbilities)
            {
                if (abilityOffCooldown.ExecuteAbility(attachedToDefendingEntity))
                {
                    CDebug.Log(CDebug.combatLog,
                        attachedToDefendingEntity.Id
                        + " (" + attachedToDefendingEntity.CurrentName
                        + ") executed ability " + abilityOffCooldown.DefendModeAbilityTemplate.NameInGame);

                    break;
                }
            }
        }

        private void OnDefendModeAbilityAdded(object sender, EAOnDefendModeAbilityAdded args)
        {
            OnNewAbility(args.DefendModeAbility);
        } 

        private void OnNewAbility(IDefendModeAbility ability)
        {
            ability.OnAbilityOffCooldown += OnAbilityOffCooldown;
            ability.OnAbilityExecuted += OnAbilityExecuted;

            if (ability.IsOffCooldown)
            {
                offCooldownAbilities.Add(ability);
            }
            else
            {
                numberOfAbilitiesOnCooldown += 1;
            }
        }

        private void OnDefendModeAbilityRemoved(object sender, EAOnDefendModeAbilityRemoved args)
        {
            args.DefendModeAbility.OnAbilityOffCooldown -= OnAbilityOffCooldown;
            args.DefendModeAbility.OnAbilityExecuted -= OnAbilityExecuted;

            if (args.DefendModeAbility.IsOffCooldown)
            {
                offCooldownAbilities.Remove(args.DefendModeAbility);
            }
            else
            {
                numberOfAbilitiesOnCooldown -= 1;
            }
        }

        private void OnAbilityOffCooldown(object sender, EAOnAbilityOffCooldown args)
        {
            offCooldownAbilities.Add(args.DefendModeAbility);

            numberOfAbilitiesOnCooldown -= 1;

            if (numberOfAbilitiesOnCooldown == 0)
            {
                OnAllDefendModeAbilitiesOffCooldown?.Invoke(this, new EAOnAllDefendModeAbilitiesOffCooldown());
            }
        }

        private void OnAbilityExecuted(object sender, EAOnAbilityExecuted args)
        {
            offCooldownAbilities.Remove(args.DefendModeAbility);

            numberOfAbilitiesOnCooldown += 1;
        }
    }
}