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
            DepsProv.TheGameStateManager.OnEnterBuildMode += (object sender, EAOnEnterBuildMode args) => modeText.text = "Mode: Build";
            DepsProv.TheGameStateManager.OnEnterDefendMode += (object sender, EAOnEnterDefendMode args) => modeText.text = "Mode: Defend";
        }
    }
}