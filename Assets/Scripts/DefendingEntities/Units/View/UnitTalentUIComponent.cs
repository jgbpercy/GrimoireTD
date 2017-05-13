using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitTalentUIComponent : MonoBehaviour {

    private Unit selectedUnit;

    private UnitTalent unitTalent;

    private bool intialised = false;

    [SerializeField]
    private Slider ownSlider;
    [SerializeField]
    private Text ownText;

    public void SetUp(Unit selectedUnit, UnitTalent unitTalent)
    {
        if (intialised)
        {
            return;
        }

        this.selectedUnit = selectedUnit;
        this.unitTalent = unitTalent;

        SetValues();

        selectedUnit.RegisterForExperienceFatigueChangedCallback(SetValues);
    }

    private void SetValues()
    {
        ownText.text = TalentText(unitTalent);

        ownSlider.maxValue = unitTalent.AttributeBonuses.Length;
        ownSlider.value = selectedUnit.LevelledTalents[unitTalent];
    }

    private string TalentText(UnitTalent talent)
    {
        return selectedUnit.LevelledTalents[talent] + " / " + talent.AttributeBonuses.Length + "\n" + talent.DescriptionText;
    }

    public void UIComponentClicked()
    {
        InterfaceController.Instance.ClickUnitTalent(selectedUnit, unitTalent);
    }

    private void OnDestroy()
    {
        if ( selectedUnit != null )
        {
            selectedUnit.DeregisterForExperienceFatigueChangedCallback(SetValues);
        }
    }
}
