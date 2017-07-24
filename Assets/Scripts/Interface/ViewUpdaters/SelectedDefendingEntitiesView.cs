using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedDefendingEntitiesView : SingletonMonobehaviour<SelectedDefendingEntitiesView> {

    [SerializeField]
    private GameObject abilitySliderPrefab;

    [SerializeField]
    private GameObject abilityButtonPrefab;

    [SerializeField]
    private GameObject selectedStructurePanel;
    [SerializeField]
    private Text selectedStructureName;
    [SerializeField]
    private Text selectedStructureText;
    [SerializeField]
    private GameObject structureAbilityVerticalLayout;

    [SerializeField]
    private GameObject selectedUnitPanel;
    [SerializeField]
    private Text selectedUnitName;
    [SerializeField]
    private Text selectedUnitText;
    [SerializeField]
    private GameObject unitAbilityVerticalLayout;

    [SerializeField]
    private Text unitExperienceText;
    [SerializeField]
    private Text unitFatigueText;
    [SerializeField]
    private Slider unitExperienceSlider;
    [SerializeField]
    private GameObject unitTalentButton;
    private Text unitTalentButtonText;

    [SerializeField]
    private Text unitLevelText;

    [SerializeField]
    private GameObject unitTalentPanel;
    [SerializeField]
    private Transform unitTalentLayout;
    [SerializeField]
    private GameObject unitTalentDisplayPrefab;

    [SerializeField]
    private GameObject structureUpgradePanel;
    [SerializeField]
    private Transform structureUpgradeLayout;
    [SerializeField]
    private GameObject structureUpgradeDisplayPrefab;
    [SerializeField]
    private GameObject structureEnhancementDisplayPrefab;

    [SerializeField]
    private Slider idleActiveSlider;
    [SerializeField]
    private Text activeText;
    [SerializeField]
    private Text idleText;

    private Unit selectedUnit = null;

    private Structure selectedStructure = null;

    private List<DefendModeAbilityUIComponent> abilitySliders = new List<DefendModeAbilityUIComponent>();
    private List<BuildModeAbilityUIComponent> abilityButtons = new List<BuildModeAbilityUIComponent>();

    private List<GameObject> talentDisplays = new List<GameObject>();

    private List<GameObject> upgradeDisplays = new List<GameObject>();
    private List<GameObject> enhancementDisplays = new List<GameObject>();

    private Action<Ability> OnStructureAbilityAdded;
    private Action<Ability> OnStructureAbilityRemoved;
    private Action<Ability> OnUnitAbilityAdded;
    private Action<Ability> OnUnitAbilityRemoved;

    private void Start()
    {
        CDebug.Log(CDebug.applicationLoading, "Selected Defending Entites View Start");

        unitTalentButtonText = unitTalentButton.GetComponentInChildren<Text>();

        selectedStructurePanel.SetActive(false);
        selectedUnitPanel.SetActive(false);

        InterfaceController.Instance.RegisterForOnDefendingEntitySelectedCallback(OnNewSelection);
        InterfaceController.Instance.RegisterForOnDefendingEntityDeselectedCallback(OnDefendingEntitiesDeselected);

        GameStateManager.Instance.RegisterForOnEnterDefendModeCallback(OnEnterDefendMode);
    }

    private void Update()
    {
        //TEMP DEBUG ADDEXPERIENCE TODO: REMOVE
        if (CDebug.debugCommands.Enabled && Input.GetAxisRaw("DebugSpawnWave") > 0)
        {
            selectedUnit.TempDebugAddExperience();
        }

        if ( GameStateManager.Instance.CurrentGameMode == GameMode.BUILD )
        {
            return;
        }

        if ( selectedUnit != null)
        {
            idleActiveSlider.maxValue = selectedUnit.TimeActive + selectedUnit.TimeIdle;
            idleActiveSlider.value = selectedUnit.TimeActive;
            activeText.text = selectedUnit.TimeActive.ToString("0.0");
            idleText.text = selectedUnit.TimeIdle.ToString("0.0");
        }
    }

    private void OnNewSelection(Structure newSelectedStructure, Unit newSelectedUnit)
    {
        ClearAbilityLists();

        if ( newSelectedStructure != null )
        {
            OnDefendingEntitySelected(newSelectedStructure, selectedStructurePanel, selectedStructureName, selectedStructureText, structureAbilityVerticalLayout);

            SetSelectedStructureNullCallbackSafe();

            selectedStructure = newSelectedStructure;

            SetUpUpgradeView();

            selectedStructure.RegisterForOnUpgradedCallback(OnStructureUpgraded);

            OnStructureAbilityAdded = new Action<Ability>(x => OnNewAbilityAdded(x, structureAbilityVerticalLayout));
            OnStructureAbilityRemoved = new Action<Ability>(x => OnAbilityRemoved(x, structureAbilityVerticalLayout));

            selectedStructure.RegisterForOnAbilityAddedCallback(OnStructureAbilityAdded);
            selectedStructure.RegisterForOnAbilityRemovedCallback(OnStructureAbilityRemoved);

            if (CDebug.structureUpgrades.Enabled)
            {
                DebugLogSelectedStructureAttributes();
            }
        }
        else
        {
            OnStructureDeselection();
        }

        if ( newSelectedUnit != null )
        {
            OnDefendingEntitySelected(newSelectedUnit, selectedUnitPanel, selectedUnitName, selectedUnitText, unitAbilityVerticalLayout);

            SetSelectedUnitNullCallbackSafe();

            selectedUnit = newSelectedUnit;

            SetExperienceFatigueLevelView();
            SetUpTalentDisplay();

            selectedUnit.RegisterForExperienceFatigueChangedCallback(OnUnitExperienceChange);

            OnUnitAbilityAdded = new Action<Ability>(x => OnNewAbilityAdded(x, unitAbilityVerticalLayout));
            OnUnitAbilityRemoved = new Action<Ability>(x => OnAbilityRemoved(x, unitAbilityVerticalLayout));

            selectedUnit.RegisterForOnAbilityAddedCallback(OnUnitAbilityAdded);
            selectedUnit.RegisterForOnAbilityRemovedCallback(OnUnitAbilityRemoved);

            if ( CDebug.unitAttributes.Enabled )
            {
                DebugLogSelectedUnitAttributes();
            }
        }
        else
        {
            OnUnitDeselection();
        }
    }

    private void OnDefendingEntitySelected(DefendingEntity newSelection, GameObject panel, Text nameText, Text descriptionText, GameObject abilityVerticalLayout)
    {
        panel.SetActive(true);

        SetNameAndDescription(newSelection, nameText, descriptionText);

        List<DefendModeAbility> entityDefendModeAbilities = newSelection.DefendModeAbilities();
        List<BuildModeAbility> entityBuildModeAbilities = newSelection.BuildModeAbilities();

        foreach (DefendModeAbility defendModeAbility in entityDefendModeAbilities)
        {
            AddDefendModeAbilitySlider(defendModeAbility, abilityVerticalLayout);
        }

        foreach (BuildModeAbility buildModeAbility in entityBuildModeAbilities)
        {
            AddBuildModeAbilityButton(buildModeAbility, abilityVerticalLayout);
        }
    }

    private void OnNewAbilityAdded(Ability ability, GameObject abilityVerticalLayout)
    {
        DefendModeAbility defendModeAbility = ability as DefendModeAbility;
        if (defendModeAbility != null)
        {
            AddDefendModeAbilitySlider(defendModeAbility, abilityVerticalLayout);
            return;
        }

        BuildModeAbility buildModeAbility = ability as BuildModeAbility;
        if (buildModeAbility != null)
        {
            AddBuildModeAbilityButton(buildModeAbility, abilityVerticalLayout);
            return;
        }

        throw new Exception("Unknown Ability class");
    }

    private void AddDefendModeAbilitySlider(DefendModeAbility defendModeAbility, GameObject abilityVerticalLayout)
    {
        DefendModeAbilityUIComponent newSlider = Instantiate(abilitySliderPrefab).GetComponent<DefendModeAbilityUIComponent>();
        newSlider.transform.SetParent(abilityVerticalLayout.transform);

        newSlider.SetUp(defendModeAbility);

        abilitySliders.Add(newSlider);
    }

    private void AddBuildModeAbilityButton(BuildModeAbility buildModeAbility, GameObject abilityVerticalLayout)
    {
        BuildModeAbilityUIComponent newButton = Instantiate(abilityButtonPrefab).GetComponent<BuildModeAbilityUIComponent>();
        newButton.transform.SetParent(abilityVerticalLayout.transform);

        newButton.SetUp(buildModeAbility);

        abilityButtons.Add(newButton);
    }

    private void OnAbilityRemoved(Ability ability, GameObject abilityVerticalLayout)
    {
        DefendModeAbility defendModeAbility = ability as DefendModeAbility;
        if (defendModeAbility != null)
        {
            RemoveDefendModeAbilitySlider(defendModeAbility, abilityVerticalLayout);
            return;
        }

        BuildModeAbility buildModeAbility = ability as BuildModeAbility;
        if (buildModeAbility != null)
        {
            RemoveBuildModeAbilityButton(buildModeAbility, abilityVerticalLayout);
            return;
        }

        throw new Exception("Unknown Ability class");
    }

    private void RemoveDefendModeAbilitySlider(DefendModeAbility defendModeAbility, GameObject abilityVerticalLayout)
    {
        DefendModeAbilityUIComponent uiComponentToRemove = abilitySliders.Find(x => x.DefendModeAbility == defendModeAbility);

        Destroy(uiComponentToRemove.gameObject);

        abilitySliders.Remove(uiComponentToRemove);
    }

    private void RemoveBuildModeAbilityButton(BuildModeAbility buildModeAbility, GameObject abilityVerticalLayout)
    {
        BuildModeAbilityUIComponent uiComponentToRemove = abilityButtons.Find(x => x.BuildModeAbility == buildModeAbility);

        Destroy(uiComponentToRemove.gameObject);

        abilityButtons.Remove(uiComponentToRemove);
    }

    private void SetNameAndDescription(DefendingEntity selectedEntity, Text nameText, Text descriptionText)
    {
        nameText.text = selectedEntity.CurrentName();
        descriptionText.text = selectedEntity.UIText();
    }

    public void OnDefendingEntitiesDeselected()
    {
        OnUnitDeselection();
        OnStructureDeselection();
    }

    private void OnStructureDeselection()
    {
        SetSelectedStructureNullCallbackSafe();

        selectedStructurePanel.SetActive(false);
        structureUpgradePanel.SetActive(false);
    }

    private void OnUnitDeselection()
    {
        ClearAbilityLists();
        ClearTalentDisplays();

        SetSelectedUnitNullCallbackSafe();

        selectedUnitPanel.SetActive(false);
        unitTalentPanel.SetActive(false);
    }

    private void SetSelectedStructureNullCallbackSafe()
    {
        if ( selectedStructure != null )
        {
            selectedStructure.DeregisterForOnUpgradedCallback(OnStructureUpgraded);
            selectedStructure.DeregisterForOnAbilityAddedCallback(OnStructureAbilityAdded);
            selectedStructure.DeregisterForOnAbilityRemovedCallback(OnStructureAbilityRemoved);
        }

        selectedStructure = null;
    }

    public void ToggleUpgradePanel()
    {
        if (structureUpgradePanel.activeSelf)
        {
            structureUpgradePanel.SetActive(false);
        }
        else
        {
            structureUpgradePanel.SetActive(true);
        }
    }

    private void SetSelectedUnitNullCallbackSafe()
    {
        if ( selectedUnit != null )
        {
            selectedUnit.DeregisterForExperienceFatigueChangedCallback(OnUnitExperienceChange);
            selectedUnit.DeregisterForOnAbilityAddedCallback(OnUnitAbilityAdded);
            selectedUnit.DeregisterForOnAbilityRemovedCallback(OnUnitAbilityRemoved);
        }

        selectedUnit = null;
    }

    private void SetUpUpgradeView()
    {
        ClearUpgradeAndEnhancementDisplays();

        foreach (IStructureUpgrade upgrade in selectedStructure.StructureTemplate.StructureUpgrades)
        {
            AddUpgradeDisplay(upgrade);

            if (!selectedStructure.UpgradesBought[upgrade])
            {
                return;
            }
        }
    }

    private void AddUpgradeDisplay(IStructureUpgrade upgrade)
    {
        GameObject newUpgradeDisplay = Instantiate(structureUpgradeDisplayPrefab) as GameObject;
        newUpgradeDisplay.transform.SetParent(structureUpgradeLayout);

        upgradeDisplays.Add(newUpgradeDisplay);

        newUpgradeDisplay.GetComponentInChildren<Text>().text = "Upgrade to " + upgrade.NewStructureName + "\n" + upgrade.BonusDescription;

        foreach (StructureEnhancement enhancement in upgrade.OptionalEnhancements)
        {
            GameObject newEnhancementDisplay = Instantiate(structureEnhancementDisplayPrefab) as GameObject;
            newEnhancementDisplay.transform.SetParent(newUpgradeDisplay.transform);

            newEnhancementDisplay.GetComponent<StructureEnhancementUIComponent>().SetUp(selectedStructure, upgrade, enhancement);

            enhancementDisplays.Add(newEnhancementDisplay);
        }
    }

    private void OnStructureUpgraded()
    {
        SetNameAndDescription(selectedStructure, selectedStructureName, selectedStructureText);

        foreach (IStructureUpgrade upgrade in selectedStructure.StructureTemplate.StructureUpgrades)
        {
            if (!selectedStructure.UpgradesBought[upgrade])
            {
                AddUpgradeDisplay(upgrade);
                return;
            }
        }
    }

    private void ClearUpgradeAndEnhancementDisplays()
    {
        enhancementDisplays.ForEach(x => Destroy(x.gameObject));
        upgradeDisplays.ForEach(x => Destroy(x.gameObject));

        enhancementDisplays = new List<GameObject>();
        upgradeDisplays = new List<GameObject>();
    }

    public void ToggleTalentPanel()
    {
        if (unitTalentPanel.activeSelf)
        {
            unitTalentPanel.SetActive(false);
        }
        else
        {
            unitTalentPanel.SetActive(true);
        }
    }

    private void SetExperienceFatigueLevelView()
    {
        unitExperienceText.text = "Experience: " + (selectedUnit.Experience - selectedUnit.UnitTemplate.ExperienceToLevelUp * selectedUnit.Level) + " / " + selectedUnit.UnitTemplate.ExperienceToLevelUp;
        unitFatigueText.text = "Fatigue: " + selectedUnit.Fatigue;
        unitExperienceSlider.maxValue = selectedUnit.UnitTemplate.ExperienceToLevelUp;
        unitExperienceSlider.value = Mathf.Min(selectedUnit.Experience - selectedUnit.Level * selectedUnit.UnitTemplate.ExperienceToLevelUp, selectedUnit.UnitTemplate.ExperienceToLevelUp);

        unitLevelText.text = "Level: " + selectedUnit.Level;

        if (selectedUnit.LevelUpsPending > 0)
        {
            unitLevelText.text += " (" + "+ " + selectedUnit.LevelUpsPending + ")";
            unitTalentButtonText.text = "Level Up!";
        }
        else
        {
            unitTalentButtonText.text = "Talents";
        }
    }

    private void SetUpTalentDisplay()
    {
        ClearTalentDisplays();

        foreach (IUnitTalent talent in selectedUnit.UnitTemplate.UnitTalents)
        {
            GameObject newTalentDisplay = Instantiate(unitTalentDisplayPrefab) as GameObject;
            newTalentDisplay.transform.SetParent(unitTalentLayout);

            newTalentDisplay.GetComponent<UnitTalentUIComponent>().SetUp(selectedUnit, talent);

            talentDisplays.Add(newTalentDisplay);
        }
    }

    private void OnEnterDefendMode()
    {
        unitTalentPanel.SetActive(false);
        structureUpgradePanel.SetActive(false);
    }

    private void OnUnitExperienceChange()
    {
        SetExperienceFatigueLevelView();
    }

    private void ClearAbilityLists()
    {
        abilitySliders.ForEach(x => Destroy(x.gameObject));
        abilityButtons.ForEach(x => Destroy(x.gameObject));

        abilitySliders = new List<DefendModeAbilityUIComponent>();
        abilityButtons = new List<BuildModeAbilityUIComponent>();
    }

    private void ClearTalentDisplays()
    {
        talentDisplays.ForEach(x => Destroy(x.gameObject));

        talentDisplays = new List<GameObject>();
    }


    private void DebugLogSelectedUnitAttributes()
    {
        foreach (AttributeName attributeName in Enum.GetValues(typeof(AttributeName)))
        {
            CDebug.Log(CDebug.unitAttributes, selectedUnit.TempDebugGetAttributeDisplayName(attributeName) + ": " + selectedUnit.GetAttribute(attributeName));
        }
    }

    private void DebugLogSelectedStructureAttributes()
    {
        foreach (AttributeName attributeName in Enum.GetValues(typeof(AttributeName)))
        {
            CDebug.Log(CDebug.structureUpgrades, selectedStructure.TempDebugGetAttributeDisplayName(attributeName) + ": " + selectedStructure.GetAttribute(attributeName));
        }
    }

}
