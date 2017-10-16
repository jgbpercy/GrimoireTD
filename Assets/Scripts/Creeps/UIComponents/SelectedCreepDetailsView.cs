using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GrimoireTD.UI;
using GrimoireTD.Attributes;
using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    public class SelectedCreepDetailsView : MonoBehaviour
    {
        [SerializeField]
        private GameObject creepDetailsPanel;

        [SerializeField]
        private Text attributesText;
        [SerializeField]
        private Text resistancesText;
        [SerializeField]
        private Text blocksText;

        private ICreep selectedCreep;

        private void Start()
        {
            InterfaceController.Instance.OnCreepSelected += OnCreepSelected;
            InterfaceController.Instance.OnCreepDeselected += OnCreepDeselected;
        }

        private void OnCreepSelected(object sender, EAOnCreepSelected args)
        {
            SetSelectedCreepNullCallbackSafe();

            selectedCreep = args.SelectedCreep;

            attributesText.text = GetAttributesText();
            resistancesText.text = GetResistancesText();
            blocksText.text = GetBlocksText();

            selectedCreep.Attributes.OnAnyAttributeChanged += OnCreepAttibutesChange;

            selectedCreep.Resistances.OnAnyResistanceChanged += OnCreepResistancesChange;
            selectedCreep.Resistances.OnAnyBlockChanged += OnCreepBlocksChange;

            selectedCreep.Attributes.Get(CreepAttrName.armorMultiplier).OnAttributeChanged += OnCreepArmorChange;
            selectedCreep.Attributes.Get(CreepAttrName.rawArmor).OnAttributeChanged += OnCreepArmorChange;
        }

        private void OnCreepDeselected(object sender, EAOnCreepDeselected args)
        {
            creepDetailsPanel.SetActive(false);

            SetSelectedCreepNullCallbackSafe();
        }

        private void SetSelectedCreepNullCallbackSafe()
        {
            if (selectedCreep != null)
            {
                selectedCreep.Attributes.OnAnyAttributeChanged -= OnCreepAttibutesChange;

                selectedCreep.Resistances.OnAnyResistanceChanged -= OnCreepResistancesChange;
                selectedCreep.Resistances.OnAnyBlockChanged -= OnCreepBlocksChange;

                selectedCreep.Attributes.Get(CreepAttrName.armorMultiplier).OnAttributeChanged -= OnCreepArmorChange;
                selectedCreep.Attributes.Get(CreepAttrName.rawArmor).OnAttributeChanged -= OnCreepArmorChange;
            }

            selectedCreep = null;
        }

        private void OnCreepAttibutesChange(object sender, EAOnAnyAttributeChanged<CreepAttrName> args)
        {
            attributesText.text = GetAttributesText();
        }

        private void OnCreepResistancesChange(object sender, EAOnAnyResistanceChanged args)
        {
            resistancesText.text = GetResistancesText();
        }

        private void OnCreepArmorChange(object sender, EAOnAttributeChanged args)
        {
            resistancesText.text = GetResistancesText();
        }

        private void OnCreepBlocksChange(object sender, EAOnAnyBlockChanged args)
        {
            blocksText.text = GetBlocksText();
        }

        private string GetAttributesText()
        {
            string attributesText = "Attributes:\n";

            foreach (KeyValuePair<CreepAttrName, string> creepAttributeName in CreepAttributeDefinitions.DisplayNames)
            {
                attributesText += 
                    creepAttributeName.Value + ": " + 
                    selectedCreep.Attributes.Get(creepAttributeName.Key).Value() +
                    "\n";
            }

            return attributesText;
        }

        private string GetResistancesText()
        {
            string resistancesText = "Resistances:\n";

            float currentCreepArmor = selectedCreep.CurrentArmor;

            foreach (IBasicMetaDamageEffectType basicMetaDamageType in GameModels.Models[0].AttackEffectTypeManager.BasicMetaDamageTypes)
            {
                resistancesText += basicMetaDamageType.ShortName + " -" +
                    " W: " + selectedCreep.Resistances.GetResistance(basicMetaDamageType.WeakMetaDamageType).Value.ToString("0.0") +
                    " B: " + selectedCreep.Resistances.GetResistance(basicMetaDamageType).Value.ToString("0.0") +
                    " S: " + selectedCreep.Resistances.GetResistance(basicMetaDamageType.StrongMetaDamageType).Value.ToString("0.0") +
                    "\n";

                foreach (ISpecificDamageEffectType specificDamageType in basicMetaDamageType.SpecificDamageTypes)
                {
                    float resistanceIncludingArmor = selectedCreep.Resistances.GetResistance(specificDamageType).Value;
                    float resistanceWithoutArmor = selectedCreep.Resistances.GetResistanceWithoutArmor(specificDamageType);

                    resistancesText += specificDamageType.ShortName + ": " +
                        resistanceIncludingArmor.ToString("0.0") + " (" +
                        resistanceWithoutArmor.ToString("0.0") + " + " +
                        (resistanceIncludingArmor - resistanceWithoutArmor).ToString("0.0") + 
                        ")\n";
                }
            }

            return resistancesText;
        }

        private string GetBlocksText()
        {
            string blocksText = "Blocks:\n";

            foreach (IBasicMetaDamageEffectType basicMetaDamageType in GameModels.Models[0].AttackEffectTypeManager.BasicMetaDamageTypes)
            {
                blocksText += basicMetaDamageType.ShortName + " -" +
                    " W: " + selectedCreep.Resistances.GetBlock(basicMetaDamageType.WeakMetaDamageType).Value +
                    " B: " + selectedCreep.Resistances.GetBlock(basicMetaDamageType).Value +
                    " S: " + selectedCreep.Resistances.GetBlock(basicMetaDamageType.StrongMetaDamageType).Value +
                    "\n";

                foreach (ISpecificDamageEffectType specificDamageType in basicMetaDamageType.SpecificDamageTypes)
                {
                    blocksText += specificDamageType.ShortName + ": " + selectedCreep.Resistances.GetBlock(specificDamageType).Value + "\n";
                }
            }

            return blocksText;
        }
    }
}