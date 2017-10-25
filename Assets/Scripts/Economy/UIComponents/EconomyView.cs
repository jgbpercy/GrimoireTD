using UnityEngine;
using UnityEngine.UI;
using GrimoireTD.Technical;
using GrimoireTD.Dependencies;

namespace GrimoireTD.Economy
{
    public class EconomyView : SingletonMonobehaviour<EconomyView>
    {
        [SerializeField]
        private Text resourceUIText;

        void Start()
        {
            DepsProv.TheEconomyManager.OnAnyResourceChanged += OnResourceValueChange;
        }

        private void OnResourceValueChange(object sender, EAOnAnyResourceChanged args)
        {
            resourceUIText.text = DepsProv.TheEconomyManager.ResourcesAsTransaction.ToString(EconomyTransactionStringFormat.FullNameLineBreaks, false);
        }
    }
}