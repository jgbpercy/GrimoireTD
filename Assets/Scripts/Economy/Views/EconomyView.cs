using UnityEngine;
using UnityEngine.UI;
using GrimoireTD.Technical;
using GrimoireTD.ChannelDebug;

namespace GrimoireTD.Economy
{
    public class EconomyView : SingletonMonobehaviour<EconomyView>
    {
        private IReadOnlyEconomyManager economyManager;

        [SerializeField]
        private Text resourceUIText;

        void Start()
        {
            CDebug.Log(CDebug.applicationLoading, "Economy View Start");

            economyManager = GameModels.Models[0].EconomyManager;

            economyManager.RegisterForOnAnyResourceChangedCallback(OnResourceValueChange);
        }

        private void OnResourceValueChange(IResource changedResource, int byAmount, int newAmount)
        {
            resourceUIText.text = economyManager.ResourcesAsTransaction.ToString(EconomyTransactionStringFormat.FullNameLineBreaks, false);
        }
    }
}