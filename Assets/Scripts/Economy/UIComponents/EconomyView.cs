﻿using UnityEngine;
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

            economyManager.OnAnyResourceChanged += OnResourceValueChange;
        }

        private void OnResourceValueChange(object sender, EAOnAnyResourceChanged args)
        {
            resourceUIText.text = economyManager.ResourcesAsTransaction.ToString(EconomyTransactionStringFormat.FullNameLineBreaks, false);
        }
    }
}