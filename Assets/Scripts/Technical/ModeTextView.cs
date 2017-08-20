using UnityEngine;
using UnityEngine.UI;
using GrimoireTD.ChannelDebug;

namespace GrimoireTD.Technical
{
    public class ModeTextView : SingletonMonobehaviour<ModeTextView>
    {
        [SerializeField]
        private Text modeText;

        private void Start()
        {
            CDebug.Log(CDebug.applicationLoading, "Mode Text View Start");

            GameModels.Models[0].GameStateManager.RegisterForOnEnterBuildModeCallback(() => { modeText.text = "Mode: Build"; });
            GameModels.Models[0].GameStateManager.RegisterForOnEnterDefendModeCallback(() => { modeText.text = "Mode: Defend"; });
        }
    }
}