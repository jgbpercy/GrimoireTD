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

            GameModels.Models[0].GameStateManager.OnEnterBuildMode += (object sender, EAOnEnterBuildMode args) => modeText.text = "Mode: Build";
            GameModels.Models[0].GameStateManager.OnEnterDefendMode += (object sender, EAOnEnterDefendMode args) => modeText.text = "Mode: Defend";
        }
    }
}