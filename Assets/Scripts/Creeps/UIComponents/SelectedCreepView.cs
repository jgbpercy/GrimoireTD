using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GrimoireTD.ChannelDebug;
using GrimoireTD.TemporaryEffects;
using GrimoireTD.UI;
using GrimoireTD.Technical;

namespace GrimoireTD.Creeps
{
    public class SelectedCreepView : SingletonMonobehaviour<SelectedCreepView>
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

        [SerializeField]
        private GameObject creepDetailsPanel;

        private List<GameObject> effectSliders;

        private ICreep selectedCreep = null;

        private void Start()
        {
            CDebug.Log(CDebug.applicationLoading, "Selected Creep View Start");

            InterfaceController.Instance.OnCreepSelected += OnNewSelection;
            InterfaceController.Instance.OnCreepDeselected += OnDeselection;

            effectSliders = new List<GameObject>();

            selectedCreepPanel.SetActive(false);
        }

        private void OnNewSelection(object sender, EAOnCreepSelected args)
        {
            ClearEffectList();
            DeregisterCallbacksWithSelectedCreep();

            selectedCreep = args.SelectedCreep;

            selectedCreepPanel.SetActive(true);

            selectedCreepName.text = selectedCreep.CreepTemplate.NameInGame;
            selectedCreepHealthBar.maxValue = selectedCreep.CreepTemplate.MaxHitpoints;
            selectedCreepHealthBar.value = selectedCreep.CurrentHitpoints;

            selectedCreepHealthText.text = selectedCreep.CurrentHitpoints + " / " + selectedCreep.CreepTemplate.MaxHitpoints;

            selectedCreep.OnHealthChanged += OnHealthChange;
            selectedCreep.OnDied += OnDied;
            selectedCreep.TemporaryEffects.OnNewTemporaryEffect += OnNewTemporaryEffect;

            foreach (ITemporaryEffect temporaryEffect in selectedCreep.TemporaryEffects.EffectList)
            {
                AddSliderForNewEffect(temporaryEffect);
            }
        }

        private void OnNewTemporaryEffect(object sender, EAOnNewTemporaryEffect args)
        {
            AddSliderForNewEffect(args.NewEffect);
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
                selectedCreep.OnHealthChanged -= OnHealthChange;
                selectedCreep.OnDied -= OnDied;
                selectedCreep.TemporaryEffects.OnNewTemporaryEffect -= OnNewTemporaryEffect;
            }
        }

        private void OnDeselection(object sender, EAOnCreepDeselected args)
        {
            ClearEffectList();

            DeregisterCallbacksWithSelectedCreep();

            selectedCreep = null;

            selectedCreepPanel.SetActive(false);
        }

        private void OnHealthChange(object sender, EAOnHealthChanged args)
        {
            selectedCreepHealthBar.value = args.NewValue;
            selectedCreepHealthText.text = args.NewValue + " / " + selectedCreep.CreepTemplate.MaxHitpoints;
        }

        private void OnDied(object sender, EventArgs args)
        {
            //TODO: needed?
            DeregisterCallbacksWithSelectedCreep();

            selectedCreep = null;

            selectedCreepName.text = selectedCreepName.text + " (dead)";

            ClearEffectList();
        }

        private void ClearEffectList()
        {
            effectSliders.ForEach(x => Destroy(x.gameObject));

            effectSliders = new List<GameObject>();
        }

        public void ToggleCreepDetailsPanel()
        {
            creepDetailsPanel.SetActive(!creepDetailsPanel.activeSelf);
        }
    }
}