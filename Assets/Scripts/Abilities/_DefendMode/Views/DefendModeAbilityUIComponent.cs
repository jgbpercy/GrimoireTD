using UnityEngine;
using UnityEngine.UI;

namespace GrimoireTD.Abilities.DefendMode
{
    public class DefendModeAbilityUIComponent : MonoBehaviour
    {
        private DefendModeAbility defendModeAbility;

        private bool initialised = false;

        [SerializeField]
        private Text ownText;
        [SerializeField]
        private Slider ownSlider;

        public DefendModeAbility DefendModeAbility
        {
            get
            {
                return defendModeAbility;
            }
        }

        public void SetUp(DefendModeAbility defendModeAbility)
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
            if (GameStateManager.Instance.CurrentGameMode == GameMode.BUILD)
            {
                return;
            }

            ownText.text = defendModeAbility.DefendModeAbilityTemplate.NameInGame + " ( " + defendModeAbility.TimeSinceExecutedClamped.ToString("0.00") + " / " + defendModeAbility.ActualCooldown.ToString("0.00") + " )";
            ownSlider.value = defendModeAbility.PercentOfCooldownPassed;
        }
    }
}