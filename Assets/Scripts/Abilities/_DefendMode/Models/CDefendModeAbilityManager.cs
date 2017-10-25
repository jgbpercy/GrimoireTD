using System;
using System.Collections.Generic;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Dependencies;

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
            DepsProv.TheModelObjectFrameUpdater().Register(ModelObjectFrameUpdate);

            numberOfAbilitiesOnCooldown = 0;

            foreach (var ability in abilities.DefendModeAbilities())
            {
                OnNewAbility(ability);
            }

            abilities.OnDefendModeAbilityAdded += OnDefendModeAbilityAdded;
            abilities.OnDefendModeAbilityRemoved += OnDefendModeAbilityRemoved;

            this.attachedToDefendingEntity = attachedToDefendingEntity;
        }

        private void ModelObjectFrameUpdate(float deltaTime)
        {
            //TODO: If all model object frame updates are in defend mode, stop doing this check everywhere and only do the action in the right mode!
            if (DepsProv.TheGameStateManager.CurrentGameMode != GameMode.DEFEND)
            {
                return;
            }

            foreach (var abilityOffCooldown in offCooldownAbilities)
            {
                if (abilityOffCooldown.ExecuteAbility(attachedToDefendingEntity))
                {
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