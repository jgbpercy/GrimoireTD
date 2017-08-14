using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GrimoireTD.ChannelDebug;
using GrimoireTD.TemporaryEffects;
using GrimoireTD.UI;

namespace GrimoireTD.Creeps
{
    public class SelectedCreepView : MonoBehaviour
    {
        [SerializeField]
        private GameObject selectedCreepPanel;
        [SerializeField]
        private Text selectedCreepName;
        [SerializeField]
        private Slider selectedCreepHealthBar;
        [SerializeField]
        private Text selectedCreepHealthText;
        [SerializeField]
        private Transform effectVerticalLayout;

        [SerializeField]
        private GameObject effectSliderPrefab;

        private List<GameObject> effectSliders;

        private Creep selectedCreep = null;

        private void Start()
        {
            CDebug.Log(CDebug.applicationLoading, "Selected Creep View Start");

            InterfaceController.Instance.RegisterForOnCreepSelectedCallback(OnNewSelection);
            InterfaceController.Instance.RegisterForOnCreepDeselectedCallback(OnDeselection);

            effectSliders = new List<GameObject>();

            selectedCreepPanel.SetActive(false);
        }

        private void OnNewSelection(Creep creep)
        {
            ClearEffectList();
            DeregisterCallbacksWithSelectedCreep();

            selectedCreep = creep;

            selectedCreepPanel.SetActive(true);

            selectedCreepName.text = selectedCreep.CreepTemplate.NameInGame;
            selectedCreepHealthBar.maxValue = selectedCreep.CreepTemplate.MaxHitpoints;
            selectedCreepHealthBar.value = selectedCreep.CurrentHitpoints;

            selectedCreepHealthText.text = selectedCreep.CurrentHitpoints + " / " + selectedCreep.CreepTemplate.MaxHitpoints;

            creep.RegisterForOnHealthChangedCallback(OnHealthChange);
            creep.RegisterForOnDiedCallback(OnDied);
            creep.TemporaryEffects.RegisterForOnNewTemporaryEffectCallback(AddSliderForNewEffect);

            foreach (TemporaryEffect temporaryEffect in creep.TemporaryEffects.EffectList)
            {
                AddSliderForNewEffect(temporaryEffect);
            }
        }

        private void AddSliderForNewEffect(IReadOnlyTemporaryEffect temporaryEffect)
        {
            GameObject newSliderGo = Instantiate(effectSliderPrefab) as GameObject;
            newSliderGo.transform.SetParent(effectVerticalLayout);

            effectSliders.Add(newSliderGo);

            newSliderGo.GetComponent<CreepTemporaryEffectUiComponent>()
                .SetUp(temporaryEffect)
                .RegiterForOnDestroyCallback(() => effectSliders.Remove(newSliderGo));
        }

        private void DeregisterCallbacksWithSelectedCreep()
        {
            if (selectedCreep != null)
            {
                selectedCreep.DeregisterForOnHealthChangedCallback(OnHealthChange);
                selectedCreep.DeregisterForOnDiedCallback(OnDied);
                selectedCreep.TemporaryEffects.DeregisterForOnNewTemporaryEffectCallback(AddSliderForNewEffect);
            }
        }

        private void OnDeselection()
        {
            ClearEffectList();

            DeregisterCallbacksWithSelectedCreep();

            selectedCreep = null;

            selectedCreepPanel.SetActive(false);
        }

        private void OnHealthChange()
        {
            selectedCreepHealthBar.value = selectedCreep.CurrentHitpoints;
            selectedCreepHealthText.text = selectedCreep.CurrentHitpoints + " / " + selectedCreep.CreepTemplate.MaxHitpoints;
        }

        private void OnDied()
        {
            selectedCreep.DeregisterForOnHealthChangedCallback(OnHealthChange);
            selectedCreep.DeregisterForOnDiedCallback(OnDied);

            selectedCreep = null;

            selectedCreepName.text = selectedCreepName.text + " (dead)";

            ClearEffectList();
        }

        private void ClearEffectList()
        {
            effectSliders.ForEach(x => Destroy(x.gameObject));

            effectSliders = new List<GameObject>();
        }
    }
}