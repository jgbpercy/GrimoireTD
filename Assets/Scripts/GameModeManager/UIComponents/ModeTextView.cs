using GrimoireTD.Dependencies;
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
            DepsProv.TheGameModeManager.OnEnterBuildMode += (object sender, EAOnEnterBuildMode args) => modeText.text = "Mode: Build";
            DepsProv.TheGameModeManager.OnEnterDefendMode += (object sender, EAOnEnterDefendMode args) => modeText.text = "Mode: Defend";
        }
    }
}