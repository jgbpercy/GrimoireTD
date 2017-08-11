﻿using UnityEngine;
using UnityEngine.UI;
using GrimoireTD.Technical;
using GrimoireTD.ChannelDebug;

namespace GrimoireTD.UI
{
    public class ModeTextView : SingletonMonobehaviour<ModeTextView>
    {
        [SerializeField]
        private Text modeText;

        private void Start()
        {
            CDebug.Log(CDebug.applicationLoading, "Mode Text View Start");

            GameStateManager.Instance.RegisterForOnEnterBuildModeCallback(() => { modeText.text = "Mode: Build"; });
            GameStateManager.Instance.RegisterForOnEnterDefendModeCallback(() => { modeText.text = "Mode: Defend"; });
        }
    }
}