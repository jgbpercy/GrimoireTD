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

        private IUnit selectedUnit = null;

        private IStructure selectedStructure = null;

        private List<DefendModeAbilityUIComponent> abilitySliders = new List<DefendModeAbilityUIComponent>();
        private List<BuildModeAbilityUIComponent> abilityButtons = new List<BuildModeAbilityUIComponent>();

        private List<GameObject> talentDisplays = new List<GameObject>();

        private List<GameObject> upgradeDisplays = new List<GameObject>();
        private List<GameObject> enhancementDisplays = new List<GameObject>();

        private EventHandler<EAOnAbilityAdded> OnStructureAbilityAdded;
        private EventHandler<EAOnAbilityRemoved> OnStructureAbilityRemoved;
        private EventHandler<EAOnAbilityAdded> OnUnitAbilityAdded;
        private EventHandler<EAOnAbilityRemoved> OnUnitAbilityRemoved;

        private void Start()
        {
            CDebug.Log(CDebug.applicationLoading, "Selected Defending Entites View Start");

            unitTalentButtonText = unitTalentButton.GetComponentInChildren<Text>();

            selectedStructurePanel.SetActive(false);
            selectedUnitPanel.SetActive(false);

            InterfaceController.Instance.OnDefendingEntitySelected += OnNewSelection;
            InterfaceController.Instance.OnDefendingEntityDeselected += OnDefendingEntitiesDeselected;

            GameModels.Models[0].GameStateManager.OnEnterDefendMode += OnEnterDefendMode;
        }

        private void Update()
        {
            //TEMP DEBUG ADDEXPERIENCE TODO: REMOVE
            if (CDebug.debugCommands.Enabled && Input.GetAxisRaw("DebugSpawnWave") > 0)
            {
                selectedUnit.TempDebugAddExperience();
            }

            if (GameModels.Models[0].GameStateManager.CurrentGameMode == GameMode.BUILD)
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

        private void OnNewSelection(object sender, EAOnDefendingEntitySelected args)
        {
            ClearAbilityLists();

            if (args.SelectedStructure != null)
            {
                OnStructureSelected(args.SelectedStructure);
            }
            else
            {
                OnStructureDeselection();
            }

            if (args.SelectedUnit != null)
            {
                OnUnitSelected(args.SelectedUnit);
            }
            else
            {
                OnUnitDeselection();
            }
        }

        private void OnStructureSelected(IStructure newSelectedStructure)
        {
            OnDefendingEntitySelected(newSelectedStructure, selectedStructurePanel, selectedStructureName, selectedStructureText, structureAbilityVerticalLayout);

            SetSelectedStructureNullCallbackSafe();

            selectedStructure = newSelectedStructure;

            SetUpUpgradeView();
            SetUpStructureDetailsView();

            selectedStructure.OnUpgraded += OnStructureUpgraded;

            OnStructureAbilityAdded = new EventHandler<EAOnAbilityAdded>((sender, args) =>
            {
                structureAbilitiesText.text = GetAbilityText(selectedStructure);
                OnNewAbilityAdded(args.Ability, structureAbilityVerticalLayout);
            });
            OnStructureAbilityRemoved = new EventHandler<EAOnAbilityRemoved>((sender, args) =>
            {
                structureAbilitiesText.text = GetAbilityText(selectedStructure);
                OnAbilityRemoved(args.Ability, structureAbilityVerticalLayout);
            });

            selectedStructure.Abilities.OnAbilityAdded += OnStructureAbilityAdded;
            selectedStructure.Abilities.OnAbilityRemoved -= OnStructureAbilityRemoved;

            selectedStructure.AurasEmitted.OnAdd += OnStructureAuraEmittedAdded;
            selectedStructure.AurasEmitted.OnRemove -= OnStructureAuraEmittedRemoved;
            selectedStructure.AffectedByDefenderAuras.OnAdd += OnStructureAuraAffectedByAdded;
            selectedStructure.AffectedByDefenderAuras.OnRemove -= OnStructureAuraAffectedByRemoved;

            selectedStructure.Attributes.OnAnyAttributeChanged += OnStructureAttributesChange;

            selectedStructure.FlatHexOccupationBonuses.OnAdd += OnStructureFlatHexOccupationBonusAdded;
            selectedStructure.FlatHexOccupationBonuses.OnRemove -= OnStructureFlatHexOccupationBonusRemoved;

            if (CDebug.structureUpgrades.Enabled)
            {
                DebugLogSelectedStructureAttributes();
            }
        }

        private void OnUnitSelected(IUnit newSelectedUnit)
        {
            OnDefendingEntitySelected(newSelectedUnit, selectedUnitPanel, selectedUnitName, selectedUnitText, unitAbilityVerticalLayout);

            SetSelectedUnitNullCallbackSafe();

            selectedUnit = newSelectedUnit;

            SetExperienceFatigueLevelView();
            SetUpTalentView();
            SetUpUnitDetailsView();

            selectedUnit.OnExperienceFatigueLevelChanged += OnUnitExperienceChange;

            OnUnitAbilityAdded = new EventHandler<EAOnAbilityAdded>((sender, args) =>
            {
                unitAbilitiesText.text = GetAbilityText(selectedUnit);
                OnNewAbilityAdded(args.Ability, unitAbilityVerticalLayout);
            });
            OnUnitAbilityRemoved = new EventHandler<EAOnAbilityRemoved>((sender, args) =>
            {
                unitAbilitiesText.text = GetAbilityText(selectedUnit);
                OnAbilityRemoved(args.Ability, unitAbilityVerticalLayout);
            });

            selectedUnit.Abilities.OnAbilityAdded += OnUnitAbilityAdded;
            selectedUnit.Abilities.OnAbilityRemoved -= OnUnitAbilityRemoved;

            selectedUnit.AurasEmitted.OnAdd += OnUnitAuraEmittedAdded;
            selectedUnit.AurasEmitted.OnRemove -= OnUnitAuraEmittedRemoved;
            selectedUnit.AffectedByDefenderAuras.OnAdd += OnUnitAffectedByAuraAdded;
            selectedUnit.AffectedByDefenderAuras.OnRemove -= OnUnitAffecteByAuraRemoved;

            selectedUnit.Attributes.OnAnyAttributeChanged += OnUnitAttributesChange;

            selectedUnit.FlatHexOccupationBonuses.OnAdd += OnUnitFlatHexOccupationBonusAdded;
            selectedUnit.FlatHexOccupationBonuses.OnRemove -= OnUnitFlatHexOccupationBonusRemoved;

            selectedUnit.ConditionalHexOccupationBonuses.OnAdd += OnUnitConditionalHexOccupationBonusAdded;
            selectedUnit.ConditionalHexOccupationBonuses.OnRemove -= OnUnitConditionalHexOccupationBonusRemoved;
            selectedUnit.ConditionalStructureOccupationBonuses.OnAdd += OnUnitConditionalStructureOccupationBonusAdded;
            selectedUnit.ConditionalStructureOccupationBonuses.OnRemove += OnUnitConditionalStructureOccupationBonusRemoved;

            if (CDebug.unitAttributes.Enabled)
            {
                DebugLogSelectedUnitAttributes();
            }
        }

        private void OnDefendingEntitySelected(IDefendingEntity newSelection, GameObject panel, Text nameText, Text descriptionText, GameObject abilityVerticalLayout)
        {
            panel.SetActive(true);

            SetNameAndDescription(newSelection, nameText, descriptionText);

            IReadOnlyList<IDefendModeAbility> entityDefendModeAbilities = newSelection.Abilities.DefendModeAbilities();
            IReadOnlyList<IBuildModeAbility> entityBuildModeAbilities = newSelection.Abilities.BuildModeAbilities();

            foreach (IDefendModeAbility defendModeAbility in entityDefendModeAbilities)
            {
                AddDefendModeAbilitySlider(defendModeAbility, abilityVerticalLayout);
            }

            foreach (IBuildModeAbility buildModeAbility in entityBuildModeAbilities)
            {
                AddBuildModeAbilityButton(buildModeAbility, abilityVerticalLayout);
            }
        }

        private void OnNewAbilityAdded(IAbility ability, GameObject abilityVerticalLayout)
        {
            IDefendModeAbility defendModeAbility = ability as IDefendModeAbility;
            if (defendModeAbility != null)
            {
                AddDefendModeAbilitySlider(defendModeAbility, abilityVerticalLayout);
                return;
            }

            IBuildModeAbility buildModeAbility = ability as IBuildModeAbility;
            if (buildModeAbility != null)
            {
                AddBuildModeAbilityButton(buildModeAbility, abilityVerticalLayout);
                return;
            }

            throw new Exception("Unknown Ability class");
        }

        private void AddDefendModeAbilitySlider(IDefendModeAbility defendModeAbility, GameObject abilityVerticalLayout)
        {
            DefendModeAbilityUIComponent newSlider = Instantiate(abilitySliderPrefab).GetComponent<DefendModeAbilityUIComponent>();
            newSlider.transform.SetParent(abilityVerticalLayout.transform);

            newSlider.SetUp(defendModeAbility);

            abilitySliders.Add(newSlider);
        }

        private void AddBuildModeAbilityButton(IBuildModeAbility buildModeAbility, GameObject abilityVerticalLayout)
        {
            BuildModeAbilityUIComponent newButton = Instantiate(abilityButtonPrefab).GetComponent<BuildModeAbilityUIComponent>();
            newButton.transform.SetParent(abilityVerticalLayout.transform);

            newButton.SetUp(buildModeAbility);

            abilityButtons.Add(newButton);
        }

        private void OnAbilityRemoved(IAbility ability, GameObject abilityVerticalLayout)
        {
            IDefendModeAbility defendModeAbility = ability as IDefendModeAbility;
            if (defendModeAbility != null)
            {
                RemoveDefendModeAbilitySlider(defendModeAbility, abilityVerticalLayout);
                return;
            }

            IBuildModeAbility buildModeAbility = ability as IBuildModeAbility;
            if (buildModeAbility != null)
            {
                RemoveBuildModeAbilityButton(buildModeAbility, abilityVerticalLayout);
                return;
            }

            throw new Exception("Unknown Ability class");
        }

        private void RemoveDefendModeAbilitySlider(IDefendModeAbility defendModeAbility, GameObject abilityVerticalLayout)
        {
            DefendModeAbilityUIComponent uiComponentToRemove = abilitySliders.Find(x => x.DefendModeAbility == defendModeAbility);

            Destroy(uiComponentToRemove.gameObject);

            abilitySliders.Remove(uiComponentToRemove);
        }

        private void RemoveBuildModeAbilityButton(IBuildModeAbility buildModeAbility, GameObject abilityVerticalLayout)
        {
            BuildModeAbilityUIComponent uiComponentToRemove = abilityButtons.Find(x => x.BuildModeAbility == buildModeAbility);

            Destroy(uiComponentToRemove.gameObject);

            abilityButtons.Remove(uiComponentToRemove);
        }

        private void SetNameAndDescription(IDefendingEntity selectedEntity, Text nameText, Text descriptionText)
        {
            nameText.text = selectedEntity.CurrentName;
            descriptionText.text = selectedEntity.UIText;
        }

        public void OnDefendingEntitiesDeselected(object sender, EAOnDefendingEntityDeselected args)
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
                selectedStructure.OnUpgraded -= OnStructureUpgraded;

                selectedStructure.Abilities.OnAbilityAdded -= OnStructureAbilityAdded;
                selectedStructure.Abilities.OnAbilityRemoved -= OnStructureAbilityRemoved;

                selectedStructure.AurasEmitted.OnAdd -= OnStructureAuraEmittedAdded;
                selectedStructure.AurasEmitted.OnRemove -= OnStructureAuraEmittedRemoved;
                selectedStructure.AffectedByDefenderAuras.OnAdd -= OnStructureAuraAffectedByAdded;
                selectedStructure.AffectedByDefenderAuras.OnRemove -= OnStructureAuraAffectedByRemoved;

                selectedStructure.Attributes.OnAnyAttributeChanged -= OnStructureAttributesChange;

                selectedStructure.FlatHexOccupationBonuses.OnAdd -= OnStructureFlatHexOccupationBonusAdded;
                selectedStructure.FlatHexOccupationBonuses.OnRemove -= OnStructureFlatHexOccupationBonusRemoved;
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
                selectedUnit.OnExperienceFatigueLevelChanged -= OnUnitExperienceChange;

                selectedUnit.Abilities.OnAbilityAdded -= OnUnitAbilityAdded;
                selectedUnit.Abilities.OnAbilityRemoved -= OnUnitAbilityRemoved;

                selectedUnit.AurasEmitted.OnAdd -= OnUnitAuraEmittedAdded;
                selectedUnit.AurasEmitted.OnRemove -= OnUnitAuraEmittedRemoved;
                selectedUnit.AffectedByDefenderAuras.OnAdd -= OnUnitAffectedByAuraAdded;
                selectedUnit.AffectedByDefenderAuras.OnRemove -= OnUnitAffecteByAuraRemoved;

                selectedUnit.Attributes.OnAnyAttributeChanged -= OnUnitAttributesChange;

                selectedUnit.FlatHexOccupationBonuses.OnAdd -= OnUnitFlatHexOccupationBonusAdded;
                selectedUnit.FlatHexOccupationBonuses.OnRemove -= OnUnitFlatHexOccupationBonusRemoved;

                selectedUnit.ConditionalHexOccupationBonuses.OnAdd -= OnUnitConditionalHexOccupationBonusAdded;
                selectedUnit.ConditionalHexOccupationBonuses.OnRemove -= OnUnitConditionalHexOccupationBonusRemoved;
                selectedUnit.ConditionalStructureOccupationBonuses.OnAdd -= OnUnitConditionalStructureOccupationBonusAdded;
                selectedUnit.ConditionalStructureOccupationBonuses.OnRemove -= OnUnitConditionalStructureOccupationBonusRemoved;
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

            foreach (IStructureEnhancement enhancement in upgrade.OptionalEnhancements)
            {
                GameObject newEnhancementDisplay = Instantiate(structureEnhancementDisplayPrefab) as GameObject;
                newEnhancementDisplay.transform.SetParent(newUpgradeDisplay.transform);

                newEnhancementDisplay.GetComponent<StructureEnhancementUIComponent>().SetUp(selectedStructure, upgrade, enhancement);

                enhancementDisplays.Add(newEnhancementDisplay);
            }
        }

        private void OnStructureUpgraded(object sender, EAOnUpgraded args)
        {
            SetNameAndDescription(selectedStructure, selectedStructureName, selectedStructureText);

            foreach (IStructureUpgrade upgrade in selectedStructure.StructureTemplate.StructureUpgrades)
            {
                if (!selectedStructure.UpgradesBought[upgrade])
                {
                    AddUpgradeDisplay(upgrade);
                    break;
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

        private string GetAbilityText(IDefendingEntity defendingEntity)
        {
            string abilityText = "Abilities:\n";

            foreach (var ability in defendingEntity.Abilities.AbilityList)
            {
                abilityText += ability.Value.UIText() + "\n";
            }

            return abilityText;
        }

        private string GetAuraText(IDefendingEntity defendingEntity)
        {
            string auraText = "Auras:\n" + "Source Of:\n";

            foreach (IDefenderAura aura in defendingEntity.AurasEmitted)
            {
                auraText += aura.UIText() + "\n";
            }

            auraText += "Affected By:\n";

            foreach (IDefenderAura aura in defendingEntity.AffectedByDefenderAuras)
            {
                auraText += aura.UIText() + "\n";
            }

            return auraText;
        }

        private string GetAttributesText(IDefendingEntity defendingEntity)
        {
            string attributesText = "Attributes:\n";

            foreach (KeyValuePair<DEAttrName,string> attributeName in DefendingEntityAttributes.DisplayNames)
            {
                attributesText += 
                    attributeName.Value + ": " + 
                    defendingEntity.Attributes.Get(attributeName.Key).Value() + 
                    "\n";
            }

            return attributesText;
        }

        private string GetUnitEconomyText(IUnit unit)
        {
            string economyText = "Economy:\n";

            economyText += "Structures:\n";

            //TODO: This will duplicate but it's fine for now
            foreach (IStructureOccupationBonus structureOccupationBonus in unit.ConditionalStructureOccupationBonuses)
            {
                economyText += structureOccupationBonus.StructureUpgradeLevel == null ? structureOccupationBonus.StructureTemplate.StartingNameInGame : structureOccupationBonus.StructureUpgradeLevel.NewStructureName + "\n";
                economyText += structureOccupationBonus.ResourceGain.ToString(EconomyTransactionStringFormat.ShortNameSingleLine, false) + "\n";
            }

            economyText += "Hexes (conditional):\n";

            foreach (IHexType hexType in GameModels.Models[0].MapData.HexTypes)
            {
                economyText += unit.GetConditionalHexOccupationBonus(hexType).ToString(EconomyTransactionStringFormat.ShortNameSingleLine, false) + "\n";
            }

            return economyText + GetStaticEconomyText(unit);
        }

        private string GetStructureEconomyText(IStructure structure)
        {
            string economyText = "Economy:\n";

            return economyText + GetStaticEconomyText(structure);
        }

        private string GetStaticEconomyText(IDefendingEntity defendingEntity)
        {
            string economyText = "Hexes (flat):\n";

            foreach (IHexType hexType in GameModels.Models[0].MapData.HexTypes)
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

        private void OnStructureAuraEmittedAdded(object sender, EAOnCallbackListAdd<IDefenderAura> args)
        {
            structureAurasText.text = GetAuraText(selectedStructure);
        }

        private void OnStructureAuraEmittedRemoved(object sender, EAOnCallbackListRemove<IDefenderAura> args)
        {
            structureAurasText.text = GetAuraText(selectedStructure);
        }

        private void OnStructureAuraAffectedByAdded(object sender, EAOnCallbackListAdd<IDefenderAura> args)
        {
            structureAurasText.text = GetAuraText(selectedStructure);
        }

        private void OnStructureAuraAffectedByRemoved(object sender, EAOnCallbackListRemove<IDefenderAura> args)
        {
            structureAurasText.text = GetAuraText(selectedStructure);
        }

        private void OnStructureAttributesChange(object sender, EAOnAnyAttributeChanged<DEAttrName> args)
        {
            structureAttributesText.text = GetAttributesText(selectedStructure);
        }

        private void OnStructureFlatHexOccupationBonusAdded(object sender, EAOnCallbackListAdd<IHexOccupationBonus> args)
        {
            structureEconomyText.text = GetStructureEconomyText(selectedStructure);
        }

        private void OnStructureFlatHexOccupationBonusRemoved(object sender, EAOnCallbackListRemove<IHexOccupationBonus> args)
        {
            structureEconomyText.text = GetStructureEconomyText(selectedStructure);
        }

        private void OnUnitAuraEmittedAdded(object sender, EAOnCallbackListAdd<IDefenderAura> args)
        {
            unitAurasText.text = GetAuraText(selectedUnit);
        }

        private void OnUnitAuraEmittedRemoved(object sender, EAOnCallbackListRemove<IDefenderAura> args)
        {
            unitAurasText.text = GetAuraText(selectedUnit);
        }

        private void OnUnitAffectedByAuraAdded(object sender, EAOnCallbackListAdd<IDefenderAura> args)
        {
            unitAurasText.text = GetAuraText(selectedUnit);
        }

        private void OnUnitAffecteByAuraRemoved(object sender, EAOnCallbackListRemove<IDefenderAura> args)
        {
            unitAurasText.text = GetAuraText(selectedUnit);
        }

        private void OnUnitAttributesChange(object sender, EAOnAnyAttributeChanged<DEAttrName> args)
        {
            unitAttributesText.text = GetAttributesText(selectedUnit);
        }

        private void OnUnitFlatHexOccupationBonusAdded(object sender, EAOnCallbackListAdd<IHexOccupationBonus> args)
        {
            OnUnitEconomyChange();
        }

        private void OnUnitFlatHexOccupationBonusRemoved(object sender, EAOnCallbackListRemove<IHexOccupationBonus> args)
        {
            OnUnitEconomyChange();
        }

        private void OnUnitConditionalHexOccupationBonusAdded(object sender, EAOnCallbackListAdd<IHexOccupationBonus> e)
        {
            OnUnitEconomyChange();
        }

        private void OnUnitConditionalHexOccupationBonusRemoved(object sender, EAOnCallbackListRemove<IHexOccupationBonus> e)
        {
            OnUnitEconomyChange();
        }

        private void OnUnitConditionalStructureOccupationBonusAdded(object sender, EAOnCallbackListAdd<IStructureOccupationBonus> e)
        {
            OnUnitEconomyChange();
        }

        private void OnUnitConditionalStructureOccupationBonusRemoved(object sender, EAOnCallbackListRemove<IStructureOccupationBonus> e)
        {
            OnUnitEconomyChange();
        }

        private void OnUnitEconomyChange()
        {
            unitEconomyText.text = GetUnitEconomyText(selectedUnit);
        }

        private void OnEnterDefendMode(object sender, EAOnEnterDefendMode args)
        {
            unitTalentPanel.SetActive(false);
            structureUpgradePanel.SetActive(false);
        }

        private void OnUnitExperienceChange(object sender, EAOnExperienceFatigueLevelChange args)
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
            foreach (DEAttrName attributeName in Enum.GetValues(typeof(DEAttrName)))
            {
                CDebug.Log(CDebug.unitAttributes, selectedUnit.Attributes.Get(attributeName).DisplayName + ": " + selectedUnit.Attributes.Get(attributeName).Value());
            }
        }

        private void DebugLogSelectedStructureAttributes()
        {
            foreach (DEAttrName attributeName in Enum.GetValues(typeof(DEAttrName)))
            {
                CDebug.Log(CDebug.structureUpgrades, selectedStructure.Attributes.Get(attributeName).DisplayName + ": " + selectedStructure.Attributes.Get(attributeName).Value());
            }
        }
    }
}