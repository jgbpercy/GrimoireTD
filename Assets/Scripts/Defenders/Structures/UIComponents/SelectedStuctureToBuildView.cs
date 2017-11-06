using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GrimoireTD.Abilities;
using GrimoireTD.Defenders.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Technical;
using GrimoireTD.UI;

namespace GrimoireTD.Defenders.Structures
{
    public class SelectedStuctureToBuildView : SingletonMonobehaviour<SelectedStuctureToBuildView>
    {
        [SerializeField]
        private GameObject selectedStructureToBuildPanel;

        [SerializeField]
        private Text selectedStructureToBuildNameHeader;

        [SerializeField]
        private Transform abilitiesVLayout;
        [SerializeField]
        private GameObject abilityTextPrefab;

        [SerializeField]
        private Transform aurasVLayout;
        [SerializeField]
        private GameObject auraTextPrefab;

        [SerializeField]
        private Text costText;

        [SerializeField]
        private Text hexOccupationBonusText;

        private IStructureTemplate selectedStructureToBuild;

        private List<Text> abilityTexts;

        private List<Text> auraTexts;

        private void Start()
        {
            InterfaceController.Instance.OnStructureToBuildSelected += OnStructureToBuildSelected;
            InterfaceController.Instance.OnStructureToBuildDeselected += OnStructureToBuildDeselected;

            abilityTexts = new List<Text>();
            auraTexts = new List<Text>();
        }

        private void OnStructureToBuildSelected(object sender, EAOnStructureToBuildSelected args)
        {
            ClearLists();

            selectedStructureToBuildPanel.SetActive(true);

            selectedStructureToBuildNameHeader.text = args.SelectedStructureTemplate.StartingNameInGame;

            costText.text = args.SelectedStructureTemplate.Cost.ToString(EconomyTransactionStringFormat.FullNameLineBreaks, true);

            //TODO when less hungover: I can make a generic version of this as a service probably
            //Involves the added things all having their own UI component, which implements a set up interface
            foreach (var ability in args.SelectedStructureTemplate.BaseCharacteristics.Abilities)
            {
                Text newAbilityText = Instantiate(abilityTextPrefab).GetComponent<Text>();
                newAbilityText.transform.SetParent(abilitiesVLayout);

                newAbilityText.text = ability.NameInGame;

                abilityTexts.Add(newAbilityText);
            }

            foreach (var aura in args.SelectedStructureTemplate.BaseCharacteristics.Auras)
            {
                Text newAuraText = Instantiate(auraTextPrefab).GetComponent<Text>();
                newAuraText.transform.SetParent(aurasVLayout);

                newAuraText.text = aura.NameInGame;

                auraTexts.Add(newAuraText);
            }

            hexOccupationBonusText.text = "";

            foreach (IHexOccupationBonus occupationBonus in args.SelectedStructureTemplate.BaseCharacteristics.FlatHexOccupationBonuses)
            {
                hexOccupationBonusText.text += occupationBonus.HexType.NameInGame + "\n";
                hexOccupationBonusText.text += occupationBonus.ResourceGain.ToString(EconomyTransactionStringFormat.ShortNameSingleLine, false) + "\n";
            }
        }

        private void OnStructureToBuildDeselected(object sender, EAOnStructureToBuildDeselected args)
        {
            selectedStructureToBuildPanel.SetActive(false);
        }

        private void ClearLists()
        {
            abilityTexts.ForEach(x => Destroy(x.gameObject));
            auraTexts.ForEach(x => Destroy(x.gameObject));

            abilityTexts = new List<Text>();
            auraTexts = new List<Text>();
        }

        public void TogglePanel()
        {
            if (selectedStructureToBuildPanel.activeSelf)
            {
                selectedStructureToBuildPanel.SetActive(false);
            }
            else
            {
                selectedStructureToBuildPanel.SetActive(true);
            }
        }
    }
}