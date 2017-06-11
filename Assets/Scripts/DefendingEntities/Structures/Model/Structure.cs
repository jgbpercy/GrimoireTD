using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Structure : DefendingEntity {

    private string currentName;
    private string currentDescription;

    private StructureTemplate structureTemplate;

    private Dictionary<StructureUpgrade, bool> upgradesBought;
    private Dictionary<StructureEnhancement, bool> enhancementsChosen;

    private Action OnUpgradedCallback;

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

    public Structure(StructureTemplate structureTemplate, Vector3 position) : base(structureTemplate)
    {
        id = IdGen.GetNextId();

        this.structureTemplate = structureTemplate;

        currentName = structureTemplate.StartingNameInGame;
        currentDescription = structureTemplate.StartingDescription;

        DefendingEntityView.Instance.CreateStructure(this, position);

        SetUpUpgradesAndEnhancements();
    }

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

    public override string CurrentName()
    {
        return currentName;
    }

    public override string UIText()
    {
        return currentDescription;
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

        List<NamedAttributeModifier> namedModifiersToApply = new List<NamedAttributeModifier>();

        namedModifiersToApply.AddRange(upgrade.MainUpgradeBonus);
        namedModifiersToApply.AddRange(chosenEnhancement.EnhancementBonuses);

        foreach (NamedAttributeModifier namedAttributeModifier in namedModifiersToApply) 
        {
            AddAttributeModifier(namedAttributeModifier.AttributeName, namedAttributeModifier.AttributeModifier);
        }

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

    public void RegisterForOnUpgradedCallback(Action callback)
    {
        OnUpgradedCallback += callback;
    }

    public void DeregisterForOnUpgradedCallback(Action callback)
    {
        OnUpgradedCallback -= callback;
    }
}
