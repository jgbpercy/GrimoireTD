using UnityEngine;
using UnityEngine.UI;

namespace GrimoireTD.Abilities.DefendMode
{
    public class DefendModeAbilityUIComponent : MonoBehaviour
    {
        private IDefendModeAbility defendModeAbility;

        private bool initialised = false;

        [SerializeField]
        private Text ownText;
        [SerializeField]
        private Slider ownSlider;

        public IDefendModeAbility DefendModeAbility
        {
            get
            {
                return defendModeAbility;
            }
        }

        public void SetUp(IDefendModeAbility defendModeAbility)
        {
            if (initialised)
            {
                return;
            }
            initialised = true;

            this.defendModeAbility = defendModeAbility;

            ownText.text = defendModeAbility.DefendModeAbilityTemplate.NameInGame + " ( " + defendModeAbility.TimeSinceExecutedClamped.ToString("0.00") + " / " + defendModeAbility.ActualCooldown.ToString("0.00") + " )";
            ownSlider.value = defendModeAbility.PercentOfCooldownPassed;
        }

        private void Update()
        {
            if (GameModels.Models[0].GameStateManager.CurrentGameMode == GameMode.BUILD)
            {
                return;
            }

            ownText.text = defendModeAbility.DefendModeAbilityTemplate.NameInGame + " ( " + defendModeAbility.TimeSinceExecutedClamped.ToString("0.00") + " / " + defendModeAbility.ActualCooldown.ToString("0.00") + " )";
            ownSlider.value = defendModeAbility.PercentOfCooldownPassed;
        }
    }
}