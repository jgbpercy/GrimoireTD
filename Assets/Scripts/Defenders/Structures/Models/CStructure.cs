using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using GrimoireTD.Defenders.DefenderEffects;
using GrimoireTD.Map;

namespace GrimoireTD.Defenders.Structures
{
    public class CStructure : CDefender, IStructure
    {
        //Template
        public IStructureTemplate StructureTemplate { get; }

        //Name and Description
        public override string CurrentName { get; protected set; }
        public string CurrentDescription { get; private set; }

        //Upgrades
        private Dictionary<IStructureUpgrade, bool> upgradesBought;
        private Dictionary<IStructureEnhancement, bool> enhancementsChosen;

        public event EventHandler<EAOnUpgraded> OnUpgraded;

        //Public properties
        //Id
        public override string Id
        {
            get
            {
                return "S-" + id;
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

        //UI
        public override string UIText
        {
            get
            {
                return CurrentDescription;
            }

            protected set
            {
                CurrentDescription = value;
            }
        }

        //Constructor
        public CStructure(IStructureTemplate structureTemplate, Coord coordPosition) : base(structureTemplate, coordPosition)
        {
            StructureTemplate = structureTemplate;

            CurrentName = structureTemplate.StartingNameInGame;
            CurrentDescription = structureTemplate.StartingDescription;

            ApplyImprovement(structureTemplate.BaseCharacteristics);

            upgradesBought = new Dictionary<IStructureUpgrade, bool>();
            enhancementsChosen = new Dictionary<IStructureEnhancement, bool>();

            foreach (IStructureUpgrade upgrade in StructureTemplate.StructureUpgrades)
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

            IDefenderImprovement combinedImprovement = upgrade.MainUpgradeBonus.CombineWith(chosenEnhancement.EnhancementBonus);

            ApplyImprovement(combinedImprovement);

            upgradesBought[upgrade] = true;
            enhancementsChosen[chosenEnhancement] = true;

            CurrentName = upgrade.NewStructureName;
            CurrentDescription = upgrade.NewStructureDescription;

            OnUpgraded?.Invoke(this, new EAOnUpgraded(upgrade, chosenEnhancement));

            return true;
        }

        //Defender Auras Affected By
        protected override void OnNewDefenderAuraInCurrentHex(object sender, EAOnCallbackListAdd<IDefenderAura> args)
        {
            if (
                args.AddedItem.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.BOTH || 
                args.AddedItem.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.STRUCTURES)
            {
                affectedByDefenderAuras.Add(args.AddedItem);
            }
        }

        protected override void OnClearDefenderAuraInCurrentHex(object sender, EAOnCallbackListRemove<IDefenderAura> args)
        {
            if (
                args.RemovedItem.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.BOTH || 
                args.RemovedItem.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.STRUCTURES)
            {
                bool wasPresent = affectedByDefenderAuras.TryRemove(args.RemovedItem);
                Assert.IsTrue(wasPresent);
            }
        }
    }
}