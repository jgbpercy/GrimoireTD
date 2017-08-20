﻿using UnityEngine.Assertions;
using System.Collections.Generic;
using System;
using GrimoireTD.DefendingEntities.DefenderEffects;
using GrimoireTD.Map;

namespace GrimoireTD.DefendingEntities.Structures
{
    public class CStructure : CDefendingEntity, IStructure
    {
        //Name and Description
        private string currentName;
        private string currentDescription;

        //Template
        private IStructureTemplate structureTemplate;

        //Upgrades
        private Dictionary<IStructureUpgrade, bool> upgradesBought;
        private Dictionary<IStructureEnhancement, bool> enhancementsChosen;

        private Action<IStructureUpgrade, IStructureEnhancement> OnUpgradedCallback;

        //Public properties
        public override string Id
        {
            get
            {
                return "S-" + id;
            }
        }

        public string CurrentDescription
        {
            get
            {
                return currentDescription;
            }
        }

        public IStructureTemplate StructureTemplate
        {
            get
            {
                return structureTemplate;
            }
        }

        public IReadOnlyDictionary<IStructureUpgrade, bool> UpgradesBought
        {
            get
            {
                return upgradesBought;
            }
        }

        public IReadOnlyDictionary<IStructureEnhancement, bool> EnhancementsChosen
        {
            get
            {
                return enhancementsChosen;
            }
        }

        //Constructor
        public CStructure(IStructureTemplate structureTemplate, Coord coordPosition) : base(structureTemplate, coordPosition)
        {
            this.structureTemplate = structureTemplate;

            currentName = structureTemplate.StartingNameInGame;
            currentDescription = structureTemplate.StartingDescription;

            ApplyImprovement(structureTemplate.BaseCharacteristics);

            SetUpUpgradesAndEnhancements();

            OnHex.RegisterForOnDefenderAuraAddedCallback(OnNewDefenderAuraInCurrentHex);
            OnHex.RegisterForOnDefenderAuraRemovedCallback(OnClearDefenderAuraInCurrentHex);
        }

        //Upgrades
        private void SetUpUpgradesAndEnhancements()
        {
            upgradesBought = new Dictionary<IStructureUpgrade, bool>();
            enhancementsChosen = new Dictionary<IStructureEnhancement, bool>();

            foreach (IStructureUpgrade upgrade in structureTemplate.StructureUpgrades)
            {
                upgradesBought.Add(upgrade, false);

                foreach (IStructureEnhancement enhancement in upgrade.OptionalEnhancements)
                {
                    enhancementsChosen.Add(enhancement, false);
                }
            }
        }

        public IStructureUpgrade CurrentUpgradeLevel()
        {
            IStructureUpgrade currentUpgrade = null;

            foreach (KeyValuePair<IStructureUpgrade, bool> structureUpgrade in upgradesBought)
            {
                if (structureUpgrade.Value == false)
                {
                    break;
                }

                currentUpgrade = structureUpgrade.Key;
            }

            return currentUpgrade;
        }

        public bool TryUpgrade(IStructureUpgrade upgrade, IStructureEnhancement chosenEnhancement)
        {
            if (upgradesBought[upgrade])
            {
                return false;
            }

            if (!chosenEnhancement.Cost.CanDoTransaction())
            {
                return false;
            }

            IDefendingEntityImprovement combinedImprovement = upgrade.MainUpgradeBonus.Combine(chosenEnhancement.EnhancementBonus);

            ApplyImprovement(combinedImprovement);

            upgradesBought[upgrade] = true;
            enhancementsChosen[chosenEnhancement] = true;

            currentName = upgrade.NewStructureName;
            currentDescription = upgrade.NewStructureDescription;

            OnUpgradedCallback?.Invoke(upgrade, chosenEnhancement);

            return true;
        }

        //Defender Auras Affected By
        protected override void OnNewDefenderAuraInCurrentHex(IDefenderAura aura)
        {
            if (aura.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.BOTH || aura.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.STRUCTURES)
            {
                affectedByDefenderAuras.Add(aura);
            }
        }

        protected override void OnClearDefenderAuraInCurrentHex(IDefenderAura aura)
        {
            if (aura.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.BOTH || aura.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.UNITS)
            {
                bool wasPresent = affectedByDefenderAuras.Contains(aura);
                Assert.IsTrue(wasPresent);

                RemoveImprovement(aura.DefenderEffectTemplate.Improvement);
            }
        }

        //UI
        public override string CurrentName()
        {
            return currentName;
        }

        public override string UIText()
        {
            return currentDescription;
        }

        //Callbacks
        public void RegisterForOnUpgradedCallback(Action<IStructureUpgrade, IStructureEnhancement> callback)
        {
            OnUpgradedCallback += callback;
        }

        public void DeregisterForOnUpgradedCallback(Action<IStructureUpgrade, IStructureEnhancement> callback)
        {
            OnUpgradedCallback -= callback;
        }
    }
}