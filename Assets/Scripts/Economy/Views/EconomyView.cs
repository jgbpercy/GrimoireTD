using UnityEngine;
using UnityEngine.UI;

public class EconomyView : SingletonMonobehaviour<EconomyView> {

    private EconomyManager economyManager;

    [SerializeField]
    private Text resourceUIText;

    void Start () {

        CDebug.Log(CDebug.applicationLoading, "Economy Manager Start");

        economyManager = EconomyManager.Instance;
        economyManager.enabled = true;

        economyManager.RegisterForOnResourceValueChangeCallback(OnResourceValueChange);
	}
	
    private string ResourceUIText()
    {
        string resourceUIText = "";

        resourceUIText += economyManager.Food.NameInGame + ": <i>" + economyManager.Food.AmountOwned + "</i>\n";
        resourceUIText += economyManager.Wood.NameInGame + ": <i>" + economyManager.Wood.AmountOwned + "</i>\n";
        resourceUIText += economyManager.Stone.NameInGame + ": <i>" + economyManager.Stone.AmountOwned + "</i>\n";
        resourceUIText += economyManager.Gold.NameInGame + ": <i>" + economyManager.Gold.AmountOwned + "</i>\n";
        resourceUIText += economyManager.Mana.NameInGame + ": <i>" + economyManager.Mana.AmountOwned + "</i>\n";

        return resourceUIText;
    }

    private void OnResourceValueChange()
    {
        resourceUIText.text = ResourceUIText();
    }

}
