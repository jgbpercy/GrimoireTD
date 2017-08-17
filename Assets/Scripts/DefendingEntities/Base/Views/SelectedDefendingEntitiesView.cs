using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GrimoireTD.Abilities;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.DefendingEntities.Units;
using GrimoireTD.DefendingEntities.Structures;
using GrimoireTD.DefendingEntities.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Technical;
using GrimoireTD.ChannelDebug;
using GrimoireTD.Map;
using GrimoireTD.Attributes;
using GrimoireTD.UI;

namespace GrimoireTD.DefendingEntities
{
    //TODO: oh god split up this class oh god
    public class SelectedDefendingEntitiesView : SingletonMonobehaviour<SelectedDefendingEntitiesView>
    {
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

        [SerializeField]
        private GameObject structureDetailsPanel;
        [SerializeField]
        private Text structureAbilitiesText;
        [SerializeField]
        private Text structureAurasText;
        [SerializeField]
        private Text structureAttributesText;
        [SerializeField]
        private Text structureEconomyText;

        [SerializeField]
        private GameObject unitDetailsPanel;
        [SerializeField]
        private Text unitAbilitiesText;
        [SerializeField]
        private Text unitAurasText;
        [SerializeField]
        private Text unitAttributesText;
        [SerializeField]
        private Text unitEconomyText;

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

            if (GameStateManager.Instance.CurrentGameMode == GameMode.BUILD)
            {
                return;
            }

            if (selectedUnit != null)
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

            if (newSelectedStructure != null)
            {
                OnStructureSelected(newSelectedStructure);
            }
            else
            {
                OnStructureDeselection();
            }

            if (newSelectedUnit != null)
            {
                OnUnitSelected(newSelectedUnit);
            }
            else
            {
                OnUnitDeselection();
            }
        }

        private void OnStructureSelected(Structure newSelectedStructure)
        {
            OnDefendingEntitySelected(newSelectedStructure, selectedStructurePanel, selectedStructureName, selectedStructureText, structureAbilityVerticalLayout);

            SetSelectedStructureNullCallbackSafe();

            selectedStructure = newSelectedStructure;

            SetUpUpgradeView();
            SetUpStructureDetailsView();

            selectedStructure.RegisterForOnUpgradedCallback(OnStructureUpgraded);

            OnStructureAbilityAdded = new Action<Ability>(x =>
            {
                structureAbilitiesText.text = GetAbilityText(selectedStructure);
                OnNewAbilityAdded(x, structureAbilityVerticalLayout);
            });
            OnStructureAbilityRemoved = new Action<Ability>(x =>
            {
                structureAbilitiesText.text = GetAbilityText(selectedStructure);
                OnAbilityRemoved(x, structureAbilityVerticalLayout);
            });

            selectedStructure.RegisterForOnAbilityAddedCallback(OnStructureAbilityAdded);
            selectedStructure.RegisterForOnAbilityRemovedCallback(OnStructureAbilityRemoved);

            selectedStructure.RegisterForOnAuraEmittedAddedCallback(OnStructureAuraChange);
            selectedStructure.RegisterForOnAuraEmittedAddedCallback(OnStructureAuraChange);
            selectedStructure.RegisterForOnAffectedByDefenderAuraAddedCallback(OnStructureAuraChange);
            selectedStructure.RegisterForOnAffectedByDefenderAuraRemovedCallback(OnStructureAuraChange);

            selectedStructure.Attributes.RegisterForOnAnyAttributeChangedCallback(OnStructureAttributesChange);

            selectedStructure.RegisterForOnFlatHexOccupationBonusAddedCallback(OnStructureEconomyChange);
            selectedStructure.RegisterForOnFlatHexOccupationBonusRemovedCallback(OnStructureEconomyChange);

            if (CDebug.structureUpgrades.Enabled)
            {
                DebugLogSelectedStructureAttributes();
            }
        }

        private void OnUnitSelected(Unit newSelectedUnit)
        {
            OnDefendingEntitySelected(newSelectedUnit, selectedUnitPanel, selectedUnitName, selectedUnitText, unitAbilityVerticalLayout);

            SetSelectedUnitNullCallbackSafe();

            selectedUnit = newSelectedUnit;

            SetExperienceFatigueLevelView();
            SetUpTalentView();
            SetUpUnitDetailsView();

            selectedUnit.RegisterForExperienceFatigueChangedCallback(OnUnitExperienceChange);

            OnUnitAbilityAdded = new Action<Ability>(x =>
            {
                unitAbilitiesText.text = GetAbilityText(selectedUnit);
                OnNewAbilityAdded(x, unitAbilityVerticalLayout);
            });
            OnUnitAbilityRemoved = new Action<Ability>(x =>
            {
                unitAbilitiesText.text = GetAbilityText(selectedUnit);
                OnAbilityRemoved(x, unitAbilityVerticalLayout);
            });

            selectedUnit.RegisterForOnAbilityAddedCallback(OnUnitAbilityAdded);
            selectedUnit.RegisterForOnAbilityRemovedCallback(OnUnitAbilityRemoved);

            selectedUnit.RegisterForOnAuraEmittedAddedCallback(OnUnitAuraChange);
            selectedUnit.RegisterForOnAuraEmittedRemovedCallback(OnUnitAuraChange);
            selectedUnit.RegisterForOnAffectedByDefenderAuraAddedCallback(OnUnitAuraChange);
            selectedUnit.RegisterForOnAffectedByDefenderAuraRemovedCallback(OnUnitAuraChange);

            selectedUnit.Attributes.RegisterForOnAnyAttributeChangedCallback(OnUnitAttributesChange);

            selectedUnit.RegisterForOnFlatHexOccupationBonusAddedCallback(OnUnitHexOccupationBonusChange);
            selectedUnit.RegisterForOnFlatHexOccupationBonusRemovedCallback(OnUnitHexOccupationBonusChange);
            selectedUnit.RegisterForOnConditionalHexOccupationBonusAddedCallback(OnUnitHexOccupationBonusChange);
            selectedUnit.RegisterForOnConditionalHexOccupationBonusRemovedCallback(OnUnitHexOccupationBonusChange);
            selectedUnit.RegisterForOnConditionalStructureOccupationBonusAddedCallback(OnUnitStuctureOccupationBonusChange);
            selectedUnit.RegisterForOnConditionalStructureOccupationBonusRemovedCallback(OnUnitStuctureOccupationBonusChange);

            if (CDebug.unitAttributes.Enabled)
            {
                DebugLogSelectedUnitAttributes();
            }
        }

        private void OnDefendingEntitySelected(DefendingEntity newSelection, GameObject panel, Text nameText, Text descriptionText, GameObject abilityVerticalLayout)
        {
            panel.SetActive(true);

            SetNameAndDescription(newSelection, nameText, descriptionText);

            IReadOnlyList<DefendModeAbility> entityDefendModeAbilities = newSelection.DefendModeAbilities();
            IReadOnlyList<BuildModeAbility> entityBuildModeAbilities = newSelection.BuildModeAbilities();

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
            structureDetailsPanel.SetActive(false);
        }

        private void OnUnitDeselection()
        {
            ClearAbilityLists();
            ClearTalentDisplays();

            SetSelectedUnitNullCallbackSafe();

            selectedUnitPanel.SetActive(false);
            unitTalentPanel.SetActive(false);
            unitDetailsPanel.SetActive(false);
        }

        private void SetSelectedStructureNullCallbackSafe()
        {
            if (selectedStructure != null)
            {
                selectedStructure.DeregisterForOnUpgradedCallback(OnStructureUpgraded);

                selectedStructure.DeregisterForOnAbilityAddedCallback(OnStructureAbilityAdded);
                selectedStructure.DeregisterForOnAbilityRemovedCallback(OnStructureAbilityRemoved);

                selectedStructure.DeregisterForOnAuraEmittedAddedCallback(OnStructureAuraChange);
                selectedStructure.DeregisterForOnAuraEmittedRemovedCallback(OnStructureAuraChange);
                selectedStructure.DeregisterForOnAffectedByDefenderAuraAddedCallback(OnStructureAuraChange);
                selectedStructure.DeregisterForOnAffectedByDefenderAuraRemovedCallback(OnStructureAuraChange);

                selectedStructure.Attributes.DeregisterForOnAnyAttributeChangedCallback(OnStructureAttributesChange);

                selectedStructure.DeregisterForOnFlatHexOccupationBonusAddedCallback(OnStructureEconomyChange);
                selectedStructure.DeregisterForOnFlatHexOccupationBonusRemovedCallback(OnStructureEconomyChange);
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
            if (selectedUnit != null)
            {
                selectedUnit.DeregisterForExperienceFatigueChangedCallback(OnUnitExperienceChange);

                selectedUnit.DeregisterForOnAbilityAddedCallback(OnUnitAbilityAdded);
                selectedUnit.DeregisterForOnAbilityRemovedCallback(OnUnitAbilityRemoved);

                selectedUnit.DeregisterForOnAuraEmittedAddedCallback(OnUnitAuraChange);
                selectedUnit.DeregisterForOnAuraEmittedRemovedCallback(OnUnitAuraChange);
                selectedUnit.DeregisterForOnAffectedByDefenderAuraAddedCallback(OnUnitAuraChange);
                selectedUnit.DeregisterForOnAffectedByDefenderAuraRemovedCallback(OnUnitAuraChange);

                selectedUnit.Attributes.DeregisterForOnAnyAttributeChangedCallback(OnUnitAttributesChange);

                selectedUnit.DeregisterForOnFlatHexOccupationBonusAddedCallback(OnUnitHexOccupationBonusChange);
                selectedUnit.DeregisterForOnFlatHexOccupationBonusRemovedCallback(OnUnitHexOccupationBonusChange);
                selectedUnit.DeregisterForOnConditionalHexOccupationBonusAddedCallback(OnUnitHexOccupationBonusChange);
                selectedUnit.DeregisterForOnConditionalHexOccupationBonusRemovedCallback(OnUnitHexOccupationBonusChange);
                selectedUnit.DeregisterForOnConditionalStructureOccupationBonusAddedCallback(OnUnitStuctureOccupationBonusChange);
                selectedUnit.DeregisterForOnConditionalStructureOccupationBonusRemovedCallback(OnUnitStuctureOccupationBonusChange);
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

        private void SetUpStructureDetailsView()
        {
            structureAbilitiesText.text = GetAbilityText(selectedStructure);
            structureAurasText.text = GetAuraText(selectedStructure);
            structureAttributesText.text = GetAttributesText(selectedStructure);
            structureEconomyText.text = GetStructureEconomyText(selectedStructure);
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

        private void SetUpTalentView()
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

        private void SetUpUnitDetailsView()
        {
            unitAbilitiesText.text = GetAbilityText(selectedUnit);
            unitAurasText.text = GetAuraText(selectedUnit);
            unitAttributesText.text = GetAttributesText(selectedUnit);
            unitEconomyText.text = GetUnitEconomyText(selectedUnit);
        }

        private string GetAbilityText(DefendingEntity defendingEntity)
        {
            string abilityText = "Abilities:\n";

            foreach (Ability ability in defendingEntity.Abilities.Values)
            {
                abilityText += ability.UIText() + "\n";
            }

            return abilityText;
        }

        private string GetAuraText(DefendingEntity defendingEntity)
        {
            string auraText = "Auras:\n" + "Source Of:\n";

            foreach (DefenderAura aura in defendingEntity.AurasEmitted)
            {
                auraText += aura.UIText() + "\n";
            }

            auraText += "Affected By:\n";

            foreach (DefenderAura aura in defendingEntity.AffectedByDefenderAuras)
            {
                auraText += aura.UIText() + "\n";
            }

            return auraText;
        }

        private string GetAttributesText(DefendingEntity defendingEntity)
        {
            string attributesText = "Attributes:\n";

            foreach (KeyValuePair<DefendingEntityAttributeName,string> attributeName in DefendingEntityAttributes.DisplayNames)
            {
                attributesText += 
                    attributeName.Value + ": " + 
                    defendingEntity.Attributes.GetAttribute(attributeName.Key) + 
                    "\n";
            }

            return attributesText;
        }

        private string GetUnitEconomyText(Unit unit)
        {
            string economyText = "Economy:\n";

            economyText += "Structures:\n";

            //TODO: This will duplicate but it's fine for now
            foreach (StructureOccupationBonus structureOccupationBonus in unit.ConditionalStructureOccupationBonuses)
            {
                economyText += structureOccupationBonus.StructureUpgradeLevel == null ? structureOccupationBonus.StructureTemplate.StartingNameInGame : structureOccupationBonus.StructureUpgradeLevel.NewStructureName + "\n";
                economyText += structureOccupationBonus.ResourceGain.ToString(EconomyTransactionStringFormat.ShortNameSingleLine, false) + "\n";
            }

            economyText += "Hexes (conditional):\n";

            foreach (IHexType hexType in MapGenerator.Instance.HexTypes)
            {
                economyText += unit.GetConditionalHexOccupationBonus(hexType).ToString(EconomyTransactionStringFormat.ShortNameSingleLine, false) + "\n";
            }

            return economyText + GetStaticEconomyText(unit);
        }

        private string GetStructureEconomyText(Structure structure)
        {
            string economyText = "Economy:\n";

            return economyText + GetStaticEconomyText(structure);
        }

        private string GetStaticEconomyText(DefendingEntity defendingEntity)
        {
            string economyText = "Hexes (flat):\n";

            foreach (IHexType hexType in MapGenerator.Instance.HexTypes)
            {
                economyText += defendingEntity.GetFlatHexOccupationBonus(hexType).ToString(EconomyTransactionStringFormat.ShortNameSingleLine, false) + "\n";
            }

            return economyText;
        }

        public void ToggleStructureDetailsPanel()
        {
            structureDetailsPanel.SetActive(!structureDetailsPanel.activeSelf);
        }

        public void ToggleUnitDetailsPanel()
        {
            unitDetailsPanel.SetActive(!unitDetailsPanel.activeSelf);
        }

        private void OnStructureAuraChange(DefenderAura aura)
        {
            structureAurasText.text = GetAuraText(selectedStructure);
        }

        private void OnStructureAttributesChange(DefendingEntityAttributeName attributeName, float newAttributeValue)
        {
            structureAttributesText.text = GetAttributesText(selectedStructure);
        }

        private void OnStructureEconomyChange(HexOccupationBonus hexOccupationBonus)
        {
            structureEconomyText.text = GetStructureEconomyText(selectedStructure);
        }

        private void OnUnitAuraChange(DefenderAura aura)
        {
            unitAurasText.text = GetAuraText(selectedUnit);
        }

        private void OnUnitAttributesChange(DefendingEntityAttributeName attributeName, float newAttrbuteValue)
        {
            unitAttributesText.text = GetAttributesText(selectedUnit);
        }

        private void OnUnitHexOccupationBonusChange(HexOccupationBonus hexOccupationBonus)
        {
            OnUnitEconomyChange();
        }

        private void OnUnitStuctureOccupationBonusChange(StructureOccupationBonus structureOccupationBonus)
        {
            OnUnitEconomyChange();
        }

        private void OnUnitEconomyChange()
        {
            unitEconomyText.text = GetUnitEconomyText(selectedUnit);
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
            foreach (DefendingEntityAttributeName attributeName in Enum.GetValues(typeof(DefendingEntityAttributeName)))
            {
                CDebug.Log(CDebug.unitAttributes, selectedUnit.Attributes.TempDebugGetAttributeDisplayName(attributeName) + ": " + selectedUnit.Attributes.GetAttribute(attributeName));
            }
        }

        private void DebugLogSelectedStructureAttributes()
        {
            foreach (DefendingEntityAttributeName attributeName in Enum.GetValues(typeof(DefendingEntityAttributeName)))
            {
                CDebug.Log(CDebug.structureUpgrades, selectedStructure.Attributes.TempDebugGetAttributeDisplayName(attributeName) + ": " + selectedStructure.Attributes.GetAttribute(attributeName));
            }
        }
    }
}