﻿using UnityEngine;
using UnityEngine.UI;
using GrimoireTD.UI;

namespace GrimoireTD.Defenders.Units
{
    public class UnitTalentUIComponent : MonoBehaviour
    {
        private IUnit selectedUnit;

        private IUnitTalent unitTalent;

        private bool intialised = false;

        [SerializeField]
        private Slider ownSlider;
        [SerializeField]
        private Text ownText;

        public void SetUp(IUnit selectedUnit, IUnitTalent unitTalent)
        {
            //TODO: can I remove all of these?
            if (intialised)
            {
                return;
            }

            this.selectedUnit = selectedUnit;
            this.unitTalent = unitTalent;

            SetValues();

            selectedUnit.OnExperienceFatigueLevelChanged += OnUnitTalentChange;
        }

        private void OnUnitTalentChange(object sender, EAOnExperienceFatigueLevelChange args)
        {
            SetValues();
        }

        private void SetValues()
        {
            ownText.text = TalentText(unitTalent);

            ownSlider.maxValue = unitTalent.UnitImprovements.Count;
            ownSlider.value = selectedUnit.TalentsLevelled[unitTalent];
        }

        private string TalentText(IUnitTalent talent)
        {
            return selectedUnit.TalentsLevelled[talent] + " / " + talent.UnitImprovements.Count + "\n" + talent.DescriptionText;
        }

        public void UIComponentClicked()
        {
            InterfaceController.Instance.ClickUnitTalent(selectedUnit, unitTalent);
        }

        private void OnDestroy()
        {
            if (selectedUnit != null)
            {
                selectedUnit.OnExperienceFatigueLevelChanged -= OnUnitTalentChange;
            }
        }
    }
}