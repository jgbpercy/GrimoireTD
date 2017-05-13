using System;
using System.Collections;
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
    private Slider idleActiveSlider;
    [SerializeField]
    private Text activeText;
    [SerializeField]
    private Text idleText;

    private Unit selectedUnit = null;

    private List<DefendModeAbility> trackedDefendModeAbilities = new List<DefendModeAbility>();
    private List<BuildModeAbility> trackedBuildModeAbilities = new List<BuildModeAbility>();

    //private List<DefendModeAbility> trackedUnitDefendModeAbilities = new List<DefendModeAbility>();
    //private List<BuildModeAbility> trackedUnitBuildModeAbilities = new List<BuildModeAbility>();

    private List<Slider> abilitySliders = new List<Slider>();
    private List<GameObject> abilityButtons = new List<GameObject>();

    private List<GameObject> talentDisplays = new List<GameObject>();

    public List<BuildModeAbility> TrackedBuildModeAbilities
    {
        get
        {
            return trackedBuildModeAbilities;
        }
    }

    private void Start()
    {
        CDebug.Log(CDebug.applicationLoading, "Selected Defending Entites View Start");

        unitTalentButtonText = unitTalentButton.GetComponentInChildren<Text>();

        InterfaceController.Instance.RegisterForOnDefendingEntitySelectedCallback(OnNewSelection);
        InterfaceController.Instance.RegisterForOnDefendingEntityDeselectedCallback(OnDefendingEntitiesDeselected);
    }

    private void Update()
    {
        //TEMP DEBUG
        if (CDebug.unitAttributes.Enabled && Input.GetAxisRaw("DebugSpawnWave") > 0)
        {
            selectedUnit.TempDebugAddExperience();
        }

        if ( GameStateManager.Instance.CurrentGameMode == GameMode.BUILD )
        {
            return;
        }

        for (int i = 0; i < abilitySliders.Count; i++)
        {
            abilitySliders[i].value = trackedDefendModeAbilities[i].PercentOfCooldownPassed;
        }

        if ( selectedUnit != null)
        {
            idleActiveSlider.maxValue = selectedUnit.TimeActive + selectedUnit.TimeIdle;
            idleActiveSlider.value = selectedUnit.TimeActive;
            activeText.text = selectedUnit.TimeActive.ToString("0.0");
            idleText.text = selectedUnit.TimeIdle.ToString("0.0");
        }

    }

    private void OnNewSelection(Structure newStructureSelected, Unit newSelectedUnit)
    {
        ClearAbilityLists();
        ClearTalentDisplays();

        if ( newStructureSelected != null )
        {
            OnDefendingEntitySelected(newStructureSelected, selectedStructurePanel, selectedStructureName, selectedStructureText, structureAbilityVerticalLayout);
        }
        else
        {
            selectedStructurePanel.SetActive(false);
        }

        if ( newSelectedUnit != null )
        {
            OnDefendingEntitySelected(newSelectedUnit, selectedUnitPanel, selectedUnitName, selectedUnitText, unitAbilityVerticalLayout);
            selectedUnit = newSelectedUnit;

            SetExperienceFatigueLevelView();
            SetUpTalentDisplay();

            selectedUnit.RegisterForExperienceFatigueChangedCallback(OnUnitExperienceChange);

            if ( CDebug.unitAttributes.Enabled )
            {
                DebugLogSelectedUnitAttributes();
            }
        }
        else
        {   
            if ( selectedUnit != null )
            {
                selectedUnit.DeregisterForExperienceFatigueChangedCallback(OnUnitExperienceChange);
                selectedUnit = null;
            }
            selectedUnitPanel.SetActive(false);
            unitTalentPanel.SetActive(false);
        }
    }

    private void DebugLogSelectedUnitAttributes()
    {
        foreach (UnitAttributeName attributeName in Enum.GetValues(typeof(UnitAttributeName)))
        {
            CDebug.Log(CDebug.unitAttributes, selectedUnit.TempDebugGetAttributeDisplayName(attributeName) + ": " + selectedUnit.GetAttribute(attributeName));
        }
    }

    private void OnDefendingEntitySelected(DefendingEntity newSelection, GameObject panel, Text nameText, Text descriptionText, GameObject abilityVerticalLayout)
    {
        panel.SetActive(true);

        nameText.text = newSelection.DefendingEntityTemplate.NameInGame;

        descriptionText.text = newSelection.UIText();

        List<DefendModeAbility> entityDefendModeAbilities = newSelection.DefendModeAbilities();
        List<BuildModeAbility> entityBuildModeAbilities = newSelection.BuildModeAbilities();

        foreach (DefendModeAbility defendModeAbility in entityDefendModeAbilities)
        {
            trackedDefendModeAbilities.Add(defendModeAbility);

            GameObject newSliderGo = Instantiate(abilitySliderPrefab) as GameObject;
            newSliderGo.transform.SetParent(abilityVerticalLayout.transform);

            Slider newSlider = newSliderGo.GetComponent<Slider>();
            abilitySliders.Add(newSlider);

            newSliderGo.GetComponentInChildren<Text>().text = defendModeAbility.DefendModeAbilityTemplate.NameInGame;
            newSlider.value = defendModeAbility.PercentOfCooldownPassed;
        }

        foreach(BuildModeAbility buildModeAbility in entityBuildModeAbilities)
        {
            trackedBuildModeAbilities.Add(buildModeAbility);

            GameObject newButtonGo = Instantiate(abilityButtonPrefab) as GameObject;
            newButtonGo.transform.SetParent(abilityVerticalLayout.transform);

            abilityButtons.Add(newButtonGo);

            newButtonGo.GetComponentInChildren<Text>().text = buildModeAbility.BuildModeAbilityTemplate.NameInGame;

            newButtonGo.GetComponentInChildren<BuildModeAbilityUIElement>().indexInAbilityList = trackedBuildModeAbilities.Count - 1;
        }
    }


    public void OnDefendingEntitiesDeselected()
    {
        ClearAbilityLists();
        ClearTalentDisplays();

        if ( selectedUnit != null )
        {
            selectedUnit.DeregisterForExperienceFatigueChangedCallback(OnUnitExperienceChange);
        }
        selectedUnit = null;

        selectedStructurePanel.SetActive(false);
        selectedUnitPanel.SetActive(false);
        unitTalentPanel.SetActive(false);
    }

    private void OnUnitExperienceChange()
    {
        SetExperienceFatigueLevelView();
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

        foreach (UnitTalent talent in selectedUnit.UnitTemplate.UnitTalents)
        {
            GameObject newTalentDisplay = Instantiate(unitTalentDisplayPrefab) as GameObject;
            newTalentDisplay.transform.SetParent(unitTalentLayout);

            newTalentDisplay.GetComponent<UnitTalentUIComponent>().SetUp(selectedUnit, talent);

            talentDisplays.Add(newTalentDisplay);
        }
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

    private void ClearAbilityLists()
    {
        abilitySliders.ForEach(x => Destroy(x.gameObject));
        abilityButtons.ForEach(x => Destroy(x.gameObject));

        abilitySliders = new List<Slider>();
        abilityButtons = new List<GameObject>();

        trackedDefendModeAbilities = new List<DefendModeAbility>();
        trackedBuildModeAbilities = new List<BuildModeAbility>();
    }

    private void ClearTalentDisplays()
    {
        talentDisplays.ForEach(x => Destroy(x));

        talentDisplays = new List<GameObject>();
    }
}
