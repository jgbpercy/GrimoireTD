using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedStructureView : SingletonMonobehaviour<SelectedStructureView> {

    [SerializeField]
    private GameObject abilityVerticalLayout;

    [SerializeField]
    private GameObject abilitySliderPrefab;

    [SerializeField]
    private GameObject selectedStructurePanel;
    [SerializeField]
    private Text selectedStructureName;
    [SerializeField]
    private Text selectedStructureText;

    private List<DefendModeAbility> trackedAbilities = new List<DefendModeAbility>();

    private List<Slider> abilitySliders = new List<Slider>();

    private void Start()
    {
        CDebug.Log(CDebug.applicationLoading, "Ability Cooldown View Start");

        InterfaceController.Instance.RegisterForOnDefendingEntitySelectedCallback(OnDefendingEntitySelected);
        InterfaceController.Instance.RegisterForOnDefendingEntityDeselectedCallback(OnDefendingEntityDeselected);
    }

    private void Update()
    {
        for (int i = 0; i < abilitySliders.Count; i++)
        {
            abilitySliders[i].value = trackedAbilities[i].PercentOfCooldownPassed;
        }
    }

    public void OnDefendingEntitySelected(DefendingEntity newSelection)
    {
        ClearAbilityLists();

        selectedStructurePanel.SetActive(true);

        Structure selectedStructureInstance = newSelection as Structure;

        selectedStructureName.text = selectedStructureInstance.StructureClassTemplate.NameInGame;

        selectedStructureText.text = selectedStructureInstance.UIText();

        trackedAbilities = newSelection.DefendModeAbilities();

        foreach (DefendModeAbility defendModeAbility in trackedAbilities)
        {
            GameObject newSliderGo = Instantiate(abilitySliderPrefab) as GameObject;
            newSliderGo.transform.SetParent(abilityVerticalLayout.transform);

            Slider newSlider = newSliderGo.GetComponent<Slider>();
            abilitySliders.Add(newSlider);

            newSliderGo.GetComponentInChildren<Text>().text = defendModeAbility.DefendModeAbilityClassTemplate.NameInGame;
            newSlider.value = defendModeAbility.PercentOfCooldownPassed;
        }
    }

    public void OnDefendingEntityDeselected()
    {
        ClearAbilityLists();
        selectedStructurePanel.SetActive(false);
    }

    private void ClearAbilityLists()
    {
        abilitySliders.ForEach(x => Destroy(x.gameObject));

        abilitySliders = new List<Slider>();

        trackedAbilities = new List<DefendModeAbility>();
    }

}
