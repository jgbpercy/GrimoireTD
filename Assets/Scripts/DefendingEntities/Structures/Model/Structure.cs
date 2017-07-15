using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using System;

public class Structure : DefendingEntity {

    //Name and Description
    private string currentName;
    private string currentDescription;

    //Template
    private StructureTemplate structureTemplate;

    //Upgrades
    private Dictionary<StructureUpgrade, bool> upgradesBought;
    private Dictionary<StructureEnhancement, bool> enhancementsChosen;

    private Action OnUpgradedCallback;

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

    public StructureTemplate StructureTemplate
    {
        get
        {
            return structureTemplate;
        }
    }

    public Dictionary<StructureUpgrade, bool> UpgradesBought
    {
        get
        {
            return upgradesBought;
        }
    }
    public Dictionary<StructureEnhancement, bool> EnhancementsChosen
    {
        get
        {
            return enhancementsChosen;
        }
    }

    //Constructor
    public Structure(StructureTemplate structureTemplate, Coord coordPosition) : base(structureTemplate, coordPosition)
    {
        this.structureTemplate = structureTemplate;

        currentName = structureTemplate.StartingNameInGame;
        currentDescription = structureTemplate.StartingDescription;

        DefendingEntityView.Instance.CreateStructure(this, coordPosition.ToPositionVector());

        ApplyImprovement(structureTemplate.BaseCharacteristics);

        SetUpUpgradesAndEnhancements();
    }

    //Upgrades
    private void SetUpUpgradesAndEnhancements()
    {
        upgradesBought = new Dictionary<StructureUpgrade, bool>();
        enhancementsChosen = new Dictionary<StructureEnhancement, bool>();

        foreach (StructureUpgrade upgrade in structureTemplate.StructureUpgrades) 
        {
            upgradesBought.Add(upgrade, false);

            foreach (StructureEnhancement enhancement in upgrade.OptionalEnhancements)
            {
                EnhancementsChosen.Add(enhancement, false);
            }
        }
    }

    public StructureUpgrade CurrentUpgradeLevel()
    {
        StructureUpgrade currentUpgrade = null;

        foreach (KeyValuePair<StructureUpgrade, bool> structureUpgrade in upgradesBought)
        {
            if (structureUpgrade.Value == false)
            {
                break;
            }

            currentUpgrade = structureUpgrade.Key;
        }

        return currentUpgrade;
    }

    public bool TryUpgrade(StructureUpgrade upgrade, StructureEnhancement chosenEnhancement, bool isFree)
    {
        if ( upgradesBought[upgrade] )
        {
            return false;
        }

        if ( !isFree && !EconomyManager.Instance.CanDoTransaction(chosenEnhancement.Cost) )
        {
            return false;
        }

        if ( !isFree )
        {
            EconomyManager.Instance.DoTransaction(chosenEnhancement.Cost);
        }
        
        IDefendingEntityImprovement combinedImprovement = upgrade.MainUpgradeBonus.Combine(chosenEnhancement.EnhancementBonus);

        ApplyImprovement(combinedImprovement);

        upgradesBought[upgrade] = true;
        enhancementsChosen[chosenEnhancement] = true;

        currentName = upgrade.NewStructureName;
        currentDescription = upgrade.NewStructureDescription;

        if ( OnUpgradedCallback != null )
        {
            OnUpgradedCallback();
        }

        return true;
    }

    //Defender Auras Affected By
    protected override void OnNewDefenderAura(DefenderAura aura)
    {
        if (aura.DefenderEffectTemplate.Affects == DefenderEffectTemplate.AffectsType.BOTH || aura.DefenderEffectTemplate.Affects == DefenderEffectTemplate.AffectsType.STRUCTURES)
        {
            affectedByDefenderAuras.Add(aura);

            ApplyImprovement(aura.DefenderEffectTemplate.Improvement);
        }
    }

    protected override void OnClearDefenderAura(DefenderAura aura)
    {
        if (aura.DefenderEffectTemplate.Affects == DefenderEffectTemplate.AffectsType.BOTH || aura.DefenderEffectTemplate.Affects == DefenderEffectTemplate.AffectsType.UNITS)
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
    public void RegisterForOnUpgradedCallback(Action callback)
    {
        OnUpgradedCallback += callback;
    }

    public void DeregisterForOnUpgradedCallback(Action callback)
    {
        OnUpgradedCallback -= callback;
    }
}
