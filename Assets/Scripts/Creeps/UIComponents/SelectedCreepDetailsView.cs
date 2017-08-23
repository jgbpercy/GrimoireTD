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
            InterfaceController.Instance.RegisterForOnCreepSelectedCallback(OnCreepSelected);
            InterfaceController.Instance.RegisterForOnCreepDeselectedCallback(OnCreepDeselected);
        }

        private void OnCreepSelected(ICreep creep)
        {
            SetSelectedCreepNullCallbackSafe();

            selectedCreep = creep;

            attributesText.text = GetAttributesText();
            resistancesText.text = GetResistancesText();
            blocksText.text = GetBlocksText();

            creep.Attributes.RegisterForOnAnyAttributeChangedCallback(OnCreepAttibutesChange);

            creep.Resistances.RegisterForOnAnyResistanceChangedCallback(OnCreepResistancesChange);
            creep.Resistances.RegisterForOnAnyBlockChangedCallback(OnCreepBlocksChange);

            creep.Attributes.RegisterForOnAttributeChangedCallback(OnCreepArmorChange, CreepAttributeName.armorMultiplier);
            creep.Attributes.RegisterForOnAttributeChangedCallback(OnCreepArmorChange, CreepAttributeName.rawArmor);
        }

        private void OnCreepDeselected()
        {
            creepDetailsPanel.SetActive(false);

            SetSelectedCreepNullCallbackSafe();
        }

        private void SetSelectedCreepNullCallbackSafe()
        {
            if (selectedCreep != null)
            {
                selectedCreep.Attributes.DeregisterForOnAnyAttributeChangedCallback(OnCreepAttibutesChange);

                selectedCreep.Resistances.DeregisterForOnAnyResistanceChangedCallback(OnCreepResistancesChange);
                selectedCreep.Resistances.DeregisterForOnAnyBlockChangeCallback(OnCreepBlocksChange);

                selectedCreep.Attributes.DeregisterForOnAttributeChangedCallback(OnCreepArmorChange, CreepAttributeName.armorMultiplier);
                selectedCreep.Attributes.DeregisterForOnAttributeChangedCallback(OnCreepArmorChange, CreepAttributeName.rawArmor);
            }

            selectedCreep = null;
        }

        private void OnCreepAttibutesChange(CreepAttributeName creepAttributeName, float value)
        {
            attributesText.text = GetAttributesText();
        }

        private void OnCreepResistancesChange(ISpecificDamageEffectType damageEffectType, float value)
        {
            resistancesText.text = GetResistancesText();
        }

        private void OnCreepArmorChange(float value)
        {
            resistancesText.text = GetResistancesText();
        }

        private void OnCreepBlocksChange(ISpecificDamageEffectType damageEffectType, int value)
        {
            blocksText.text = GetBlocksText();
        }

        private string GetAttributesText()
        {
            string attributesText = "Attributes:\n";

            foreach (KeyValuePair<CreepAttributeName,string> creepAttributeName in CreepAttributes.DisplayNames)
            {
                attributesText += 
                    creepAttributeName.Value + ": " + 
                    selectedCreep.Attributes.GetAttribute(creepAttributeName.Key) +
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
                    " W: " + selectedCreep.Resistances.GetResistance(basicMetaDamageType.WeakMetaDamageType, currentCreepArmor).ToString("0.0") +
                    " B: " + selectedCreep.Resistances.GetResistance(basicMetaDamageType, currentCreepArmor).ToString("0.0") +
                    " S: " + selectedCreep.Resistances.GetResistance(basicMetaDamageType.StrongMetaDamageType, currentCreepArmor).ToString("0.0") +
                    "\n";

                foreach (ISpecificDamageEffectType specificDamageType in basicMetaDamageType.SpecificDamageTypes)
                {
                    float resistanceIncludingArmor = selectedCreep.Resistances.GetResistance(specificDamageType, currentCreepArmor);
                    float resistanceWithoutArmor = selectedCreep.Resistances.GetResistance(specificDamageType, 0);

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
                    " W: " + selectedCreep.Resistances.GetBlock(basicMetaDamageType.WeakMetaDamageType) +
                    " B: " + selectedCreep.Resistances.GetBlock(basicMetaDamageType) +
                    " S: " + selectedCreep.Resistances.GetBlock(basicMetaDamageType.StrongMetaDamageType) +
                    "\n";

                foreach (ISpecificDamageEffectType specificDamageType in basicMetaDamageType.SpecificDamageTypes)
                {
                    blocksText += specificDamageType.ShortName + ": " + selectedCreep.Resistances.GetBlock(specificDamageType);
                }
            }

            return blocksText;
        }
    }
}