using UnityEngine.Assertions;
using System.Collections.Generic;
using System;

public class Structure : DefendingEntity {

    //Name and Description
    private string currentName;
    private string currentDescription;

    //Template
    private IStructureTemplate structureTemplate;

    //Upgrades
    private Dictionary<IStructureUpgrade, bool> upgradesBought;
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

    public IReadOnlyDictionary<StructureEnhancement, bool> EnhancementsChosen
    {
        get
        {
            return enhancementsChosen;
        }
    }

    //Constructor
    public Structure(IStructureTemplate structureTemplate, Coord coordPosition) : base(structureTemplate, coordPosition)
    {
        this.structureTemplate = structureTemplate;

        currentName = structureTemplate.StartingNameInGame;
        currentDescription = structureTemplate.StartingDescription;

        DefendingEntityView.Instance.CreateStructure(this, coordPosition.ToPositionVector());

        ApplyImprovement(structureTemplate.BaseCharacteristics);

        SetUpUpgradesAndEnhancements();

        OnHex.RegisterForOnDefenderAuraAddedCallback(OnNewDefenderAuraInCurrentHex);
        OnHex.RegisterForOnDefenderAuraRemovedCallback(OnClearDefenderAuraInCurrentHex);
    }

    //Upgrades
    private void SetUpUpgradesAndEnhancements()
    {
        upgradesBought = new Dictionary<IStructureUpgrade, bool>();
        enhancementsChosen = new Dictionary<StructureEnhancement, bool>();

        foreach (IStructureUpgrade upgrade in structureTemplate.StructureUpgrades) 
        {
            upgradesBought.Add(upgrade, false);

            foreach (StructureEnhancement enhancement in upgrade.OptionalEnhancements)
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

    public bool TryUpgrade(IStructureUpgrade upgrade, StructureEnhancement chosenEnhancement, bool isFree)
    {
        if ( upgradesBought[upgrade] )
        {
            return false;
        }

        if ( !isFree && !chosenEnhancement.Cost.CanDoTransaction() )
        {
            return false;
        }

        if ( !isFree )
        {
            chosenEnhancement.Cost.DoTransaction();
        }
        
        IDefendingEntityImprovement combinedImprovement = upgrade.MainUpgradeBonus.Combine(chosenEnhancement.EnhancementBonus);

        ApplyImprovement(combinedImprovement);

        upgradesBought[upgrade] = true;
        enhancementsChosen[chosenEnhancement] = true;

        currentName = upgrade.NewStructureName;
        currentDescription = upgrade.NewStructureDescription;

        OnUpgradedCallback?.Invoke();

        return true;
    }

    //Defender Auras Affected By
    protected override void OnNewDefenderAuraInCurrentHex(DefenderAura aura)
    {
        if (aura.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.BOTH || aura.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.STRUCTURES)
        {
            affectedByDefenderAuras.Add(aura);
        }
    }

    protected override void OnClearDefenderAuraInCurrentHex(DefenderAura aura)
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
    public void RegisterForOnUpgradedCallback(Action callback)
    {
        OnUpgradedCallback += callback;
    }

    public void DeregisterForOnUpgradedCallback(Action callback)
    {
        OnUpgradedCallback -= callback;
    }
}
