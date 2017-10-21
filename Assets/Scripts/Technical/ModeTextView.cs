using UnityEngine;
using UnityEngine.UI;

namespace GrimoireTD.Technical
{
    public class ModeTextView : SingletonMonobehaviour<ModeTextView>
    {
        [SerializeField]
        private Text modeText;

        private void Start()
        {
            GameModels.Models[0].GameStateManager.OnEnterBuildMode += (object sender, EAOnEnterBuildMode args) => modeText.text = "Mode: Build";
            GameModels.Models[0].GameStateManager.OnEnterDefendMode += (object sender, EAOnEnterDefendMode args) => modeText.text = "Mode: Defend";
        }
    }
}