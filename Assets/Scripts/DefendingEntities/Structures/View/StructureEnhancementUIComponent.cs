using UnityEngine;
using UnityEngine.UI;

public class StructureEnhancementUIComponent : MonoBehaviour {

    private Structure selectedStructure;

    private StructureUpgrade upgrade;

    private StructureEnhancement enhancement;

    private bool initialised = false;

    [SerializeField]
    private Text ownText;
    [SerializeField]
    private Text ownCostText;
    [SerializeField]
    private Color boughtColor;
    [SerializeField]
    private Color notBoughtColor;

    public void SetUp(Structure structure, StructureUpgrade upgrade, StructureEnhancement enhancement)
    {
        if (initialised)
        {
            return;
        }
        initialised = true;

        selectedStructure = structure;
        this.upgrade = upgrade;
        this.enhancement = enhancement;

        SetDisplay();

        selectedStructure.RegisterForOnUpgradedCallback(SetDisplay);
    }

    private void SetDisplay()
    {
        ownText.text = enhancement.DescriptionText;
        ownCostText.text = enhancement.Cost.ToString(EconomyTransaction.StringFormats.ShortNameLineBreaks, true);

        if ( selectedStructure.EnhancementsChosen[enhancement] == true )
        {
            gameObject.GetComponent<Image>().color = boughtColor;
        }
        else
        {
            gameObject.GetComponent<Image>().color = notBoughtColor;
        }
    }

    public void UIComponentClicked()
    {
        InterfaceController.Instance.ClickStructureEnhancement(selectedStructure, upgrade, enhancement);
    }

    private void OnDestroy()
    {
        if ( selectedStructure != null )
        {
            selectedStructure.DeregisterForOnUpgradedCallback(SetDisplay);
        }
    }
}
