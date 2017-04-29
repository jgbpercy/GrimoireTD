using UnityEngine;
using UnityEngine.UI;

public class SelectedCreepView : MonoBehaviour {

    [SerializeField]
    private GameObject selectedCreepPanel;
    [SerializeField]
    private Text selectedCreepName;
    [SerializeField]
    private Slider selectedCreepHealthBar;
    [SerializeField]
    private Text selectedCreepHealthText;

    private Creep selectedCreep = null;

    private void Start()
    {
        CDebug.Log(CDebug.applicationLoading, "Selected Creep View Start");

        InterfaceController.Instance.RegisterForOnCreepSelectedCallback(OnNewSelection);
        InterfaceController.Instance.RegisterForOnCreepDeselectedCallback(OnDeselection);

        selectedCreepPanel.SetActive(false);
    }

    private void OnNewSelection(Creep creep)
    {
        if ( selectedCreep != null )
        {
            selectedCreep.DeregisterForOnHealthChangedCallback(OnHealthChange);
            selectedCreep.DeregisterForOnDiedCallback(OnDied);
        }

        selectedCreep = creep;

        selectedCreepPanel.SetActive(true);

        selectedCreepName.text = selectedCreep.CreepClassTemplate.NameInGame;
        selectedCreepHealthBar.maxValue = selectedCreep.CreepClassTemplate.MaxHitpoints;
        selectedCreepHealthBar.value = selectedCreep.CurrentHitpoints;

        selectedCreepHealthText.text = selectedCreep.CurrentHitpoints  + " / " + selectedCreep.CreepClassTemplate.MaxHitpoints;

        creep.RegisterForOnHealthChangedCallback(OnHealthChange);
        creep.RegisterForOnDiedCallback(OnDied);
    }

    private void OnDeselection()
    {
        if ( selectedCreep != null)
        {
            selectedCreep.DeregisterForOnHealthChangedCallback(OnHealthChange);
            selectedCreep.DeregisterForOnDiedCallback(OnDied);
        }

        selectedCreepPanel.SetActive(false);

        selectedCreep = null;
    }

    private void OnHealthChange()
    {
        selectedCreepHealthBar.value = selectedCreep.CurrentHitpoints;
        selectedCreepHealthText.text = selectedCreep.CurrentHitpoints + " / " + selectedCreep.CreepClassTemplate.MaxHitpoints;
    }

    private void OnDied()
    {
        selectedCreep.DeregisterForOnHealthChangedCallback(OnHealthChange);
        selectedCreep.DeregisterForOnDiedCallback(OnDied);

        selectedCreep = null;

        selectedCreepName.text = selectedCreepName.text + " (dead)";
    }
}
