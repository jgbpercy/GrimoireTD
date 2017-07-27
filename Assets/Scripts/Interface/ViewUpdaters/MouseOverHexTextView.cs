using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO: make this and MouseOverMapView work on callbacks rather than polling each frame
public class MouseOverHexTextView : SingletonMonobehaviour<MouseOverHexTextView> {

    private InterfaceController interfaceController;

    [SerializeField]
    private GameObject mouseOverHexPanel;

    [SerializeField]
    private Text hexTypeText;
    [SerializeField]
    private Text unitText;
    [SerializeField]
    private Text structureText;
    [SerializeField]
    private Text aurasText;

    private void Start()
    {
        interfaceController = InterfaceController.Instance;
    }

    private void Update()
    {
        bool mouseRaycastHitMap = interfaceController.MouseRaycastHitMap;

        if (!mouseRaycastHitMap)
        {
            mouseOverHexPanel.SetActive(false);
            return;
        }

        mouseOverHexPanel.SetActive(true);

        HexData mouseOverHex = interfaceController.MouseOverHex;

        hexTypeText.text = mouseOverHex.HexType.NameInGame;

        if (mouseOverHex.UnitHere != null)
        {
            unitText.text = mouseOverHex.UnitHere.CurrentName();
        }
        else
        {
            unitText.text = "None";
        }

        if (mouseOverHex.StructureHere != null)
        {
            structureText.text = mouseOverHex.StructureHere.CurrentName();
        }
        else
        {
            structureText.text = "None";
        }

        if ( mouseOverHex.DefenderAurasHere.Count > 0 )
        {
            aurasText.text = "";
            foreach (DefenderAura aura in mouseOverHex.DefenderAurasHere)
            {
                aurasText.text += aura.DefenderAuraTemplate.NameInGame + "\n";
            }
        }
        else
        {
            aurasText.text = "None";
        }
    }
}
