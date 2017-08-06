using UnityEngine;
using UnityEngine.UI;

public class EconomyView : SingletonMonobehaviour<EconomyView> {

    private EconomyManager economyManager;

    [SerializeField]
    private Text resourceUIText;

    //TODO: stop this enabling the Economy Manager
    void Start () {

        CDebug.Log(CDebug.applicationLoading, "Economy Manager Start");

        economyManager = EconomyManager.Instance;
        economyManager.enabled = true;

        economyManager.RegisterForOnAnyResourceChangedCallback(OnResourceValueChange);
	}

    private void OnResourceValueChange(IResource changedResource, int byAmount, int newAmount)
    {
        resourceUIText.text = economyManager.ResourcesAsTransaction.ToString(EconomyTransactionStringFormat.FullNameLineBreaks, false);
    }
}
