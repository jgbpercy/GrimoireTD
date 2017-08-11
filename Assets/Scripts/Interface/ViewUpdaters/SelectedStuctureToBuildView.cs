using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GrimoireTD.Abilities;
using GrimoireTD.DefendingEntities.Structures;
using GrimoireTD.DefendingEntities.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Technical;

namespace GrimoireTD.UI
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
            InterfaceController.Instance.RegisterForOnStructureToBuildSelectedCallback(OnStructureToBuildSelected);
            InterfaceController.Instance.RegisterForOnStructureToBuildDeselectedCallback(OnStructureToBuildDeselected);

            abilityTexts = new List<Text>();
            auraTexts = new List<Text>();
        }

        private void OnStructureToBuildSelected(IStructureTemplate structureTemplate)
        {
            ClearLists();

            selectedStructureToBuildPanel.SetActive(true);

            selectedStructureToBuildNameHeader.text = structureTemplate.StartingNameInGame;

            costText.text = structureTemplate.Cost.ToString(EconomyTransactionStringFormat.FullNameLineBreaks, true);

            //TODO when less hungover: I can make a generic version of this as a service probably
            //Involves the added things all having their own UI component, which implements a set up interface
            foreach (IAbilityTemplate ability in structureTemplate.BaseCharacteristics.Abilities)
            {
                Text newAbilityText = Instantiate(abilityTextPrefab).GetComponent<Text>();
                newAbilityText.transform.SetParent(abilitiesVLayout);

                newAbilityText.text = ability.NameInGame;

                abilityTexts.Add(newAbilityText);
            }

            foreach (IDefenderAuraTemplate aura in structureTemplate.BaseCharacteristics.Auras)
            {
                Text newAuraText = Instantiate(auraTextPrefab).GetComponent<Text>();
                newAuraText.transform.SetParent(aurasVLayout);

                newAuraText.text = aura.NameInGame;

                auraTexts.Add(newAuraText);
            }

            hexOccupationBonusText.text = "";

            foreach (HexOccupationBonus occupationBonus in structureTemplate.BaseCharacteristics.FlatHexOccupationBonuses)
            {
                hexOccupationBonusText.text += occupationBonus.HexType.NameInGame + "\n";
                hexOccupationBonusText.text += occupationBonus.ResourceGain.ToString(EconomyTransactionStringFormat.ShortNameSingleLine, false) + "\n";
            }
        }

        private void OnStructureToBuildDeselected()
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