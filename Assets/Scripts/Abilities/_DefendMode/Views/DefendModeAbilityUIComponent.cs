using UnityEngine;
using UnityEngine.UI;

public class DefendModeAbilityUIComponent : MonoBehaviour {

    private DefendModeAbility defendModeAbility;

    private bool initialised = false;

    [SerializeField]
    private Text ownText;
    [SerializeField]
    private Slider ownSlider;

    public void SetUp(DefendModeAbility defendModeAbility)
    {
        if (initialised)
        {
            return;
        }
        initialised = true;

        this.defendModeAbility = defendModeAbility;

        ownText.text = defendModeAbility.DefendModeAbilityTemplate.NameInGame;

        ownSlider.value = defendModeAbility.PercentOfCooldownPassed;
    }

    private void Update()
    {
        if (GameStateManager.Instance.CurrentGameMode == GameMode.BUILD)
        {
            return;
        }

        ownSlider.value = defendModeAbility.PercentOfCooldownPassed;
    }

}
