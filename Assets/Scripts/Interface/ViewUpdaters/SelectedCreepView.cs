using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GrimoireTD.Creeps;
using GrimoireTD.ChannelDebug;

namespace GrimoireTD.UI
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

        private List<Slider> effectSliders;
        private List<Text> effectSliderTexts;

        private Creep selectedCreep = null;

        private void Start()
        {
            CDebug.Log(CDebug.applicationLoading, "Selected Creep View Start");

            InterfaceController.Instance.RegisterForOnCreepSelectedCallback(OnNewSelection);
            InterfaceController.Instance.RegisterForOnCreepDeselectedCallback(OnDeselection);

            effectSliders = new List<Slider>();
            effectSliderTexts = new List<Text>();

            selectedCreepPanel.SetActive(false);
        }

        private void Update()
        {
            if (selectedCreep != null)
            {
                for (int i = 0; i < effectSliders.Count; i++)
                {
                    effectSliders[i].maxValue = selectedCreep.PersistentEffects[i].Duration;
                    effectSliders[i].value = Mathf.Max(selectedCreep.PersistentEffects[i].TimeRemaining(), 0f);
                    effectSliderTexts[i].text = selectedCreep.PersistentEffects[i].EffectName + " - " + selectedCreep.PersistentEffects[i].Magnitude + " - " + selectedCreep.PersistentEffects[i].TimeRemaining().ToString("0.0");
                }
            }
        }

        private void OnNewSelection(Creep creep)
        {
            ClearEffectLists();
            DeregisterCallbacksWithSelectedCreep();

            selectedCreep = creep;

            selectedCreepPanel.SetActive(true);

            selectedCreepName.text = selectedCreep.CreepTemplate.NameInGame;
            selectedCreepHealthBar.maxValue = selectedCreep.CreepTemplate.MaxHitpoints;
            selectedCreepHealthBar.value = selectedCreep.CurrentHitpoints;

            selectedCreepHealthText.text = selectedCreep.CurrentHitpoints + " / " + selectedCreep.CreepTemplate.MaxHitpoints;

            creep.RegisterForOnHealthChangedCallback(OnHealthChange);
            creep.RegisterForOnDiedCallback(OnDied);
            creep.RegisterForOnNewPersistentEffectCallback(AddSliderForNewEffect);
            creep.RegisterForOnPersistentEffectExpiredCallback(OnEffectExpired);

            foreach (Creep.PersistentEffect persistentEffect in creep.PersistentEffects)
            {
                AddSliderForNewEffect(persistentEffect);
            }
        }

        private void AddSliderForNewEffect(Creep.PersistentEffect persistentEffect)
        {
            GameObject newSliderGo = Instantiate(effectSliderPrefab) as GameObject;
            newSliderGo.transform.SetParent(effectVerticalLayout);

            Slider newSlider = newSliderGo.GetComponent<Slider>();
            effectSliders.Add(newSlider);

            Text newText = newSliderGo.GetComponentInChildren<Text>();
            effectSliderTexts.Add(newText);

            newText.text = persistentEffect.EffectName + " - " + persistentEffect.Magnitude + " - " + persistentEffect.TimeRemaining().ToString("0.0");
            newSlider.maxValue = persistentEffect.Duration;
            newSlider.value = persistentEffect.Duration - persistentEffect.Elapsed;
        }

        private void DeregisterCallbacksWithSelectedCreep()
        {
            if (selectedCreep != null)
            {
                selectedCreep.DeregisterForOnHealthChangedCallback(OnHealthChange);
                selectedCreep.DeregisterForOnDiedCallback(OnDied);
                selectedCreep.DeregisterForOnNewPersistentEffectCallback(AddSliderForNewEffect);
                selectedCreep.DeregisterForOnPersistentEffectExpiredCallback(OnEffectExpired);
            }
        }

        private void OnDeselection()
        {
            ClearEffectLists();

            DeregisterCallbacksWithSelectedCreep();

            selectedCreep = null;

            selectedCreepPanel.SetActive(false);
        }

        private void OnHealthChange()
        {
            selectedCreepHealthBar.value = selectedCreep.CurrentHitpoints;
            selectedCreepHealthText.text = selectedCreep.CurrentHitpoints + " / " + selectedCreep.CreepTemplate.MaxHitpoints;
        }

        private void OnEffectExpired(int index)
        {
            Destroy(effectSliders[index].gameObject);
            effectSliders.RemoveAt(index);
            effectSliderTexts.RemoveAt(index);
        }

        private void OnDied()
        {
            selectedCreep.DeregisterForOnHealthChangedCallback(OnHealthChange);
            selectedCreep.DeregisterForOnDiedCallback(OnDied);

            selectedCreep = null;

            selectedCreepName.text = selectedCreepName.text + " (dead)";

            ClearEffectLists();
        }

        private void ClearEffectLists()
        {
            effectSliders.ForEach(x => Destroy(x.gameObject));

            effectSliders = new List<Slider>();
            effectSliderTexts = new List<Text>();
        }
    }
}