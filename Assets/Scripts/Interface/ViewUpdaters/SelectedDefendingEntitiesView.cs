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

    private List<DefendModeAbility> trackedDefendModeAbilities = new List<DefendModeAbility>();
    private List<BuildModeAbility> trackedBuildModeAbilities = new List<BuildModeAbility>();

    //private List<DefendModeAbility> trackedUnitDefendModeAbilities = new List<DefendModeAbility>();
    //private List<BuildModeAbility> trackedUnitBuildModeAbilities = new List<BuildModeAbility>();

    private List<Slider> abilitySliders = new List<Slider>();
    private List<GameObject> abilityButtons = new List<GameObject>();

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

        InterfaceController.Instance.RegisterForOnDefendingEntitySelectedCallback(OnNewSelection);
        InterfaceController.Instance.RegisterForOnDefendingEntityDeselectedCallback(OnDefendingEntitiesDeselected);
    }

    private void Update()
    {
        for (int i = 0; i < abilitySliders.Count; i++)
        {
            abilitySliders[i].value = trackedDefendModeAbilities[i].PercentOfCooldownPassed;
        }
    }

    private void OnNewSelection(Structure structureSelected, Unit unitSelected)
    {
        ClearAbilityLists();

        if ( structureSelected != null )
        {
            OnDefendingEntitySelected(structureSelected, selectedStructurePanel, selectedStructureName, selectedStructureText, structureAbilityVerticalLayout);
        }
        else
        {
            selectedStructurePanel.SetActive(false);
        }

        if ( unitSelected != null )
        {
            OnDefendingEntitySelected(unitSelected, selectedUnitPanel, selectedUnitName, selectedUnitText, unitAbilityVerticalLayout);
        }
        else
        {
            selectedUnitPanel.SetActive(false);
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
        selectedStructurePanel.SetActive(false);
        selectedUnitPanel.SetActive(false);
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
}
