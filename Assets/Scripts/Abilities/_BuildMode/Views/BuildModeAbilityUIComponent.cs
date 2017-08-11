using UnityEngine;
using UnityEngine.UI;
using GrimoireTD.UI;

namespace GrimoireTD.Abilities.BuildMode
{
    public class BuildModeAbilityUIComponent : MonoBehaviour
    {
        private BuildModeAbility buildModeAbility;

        private bool initialised = false;

        [SerializeField]
        private Text ownText;

        public BuildModeAbility BuildModeAbility
        {
            get
            {
                return buildModeAbility;
            }
        }

        public void SetUp(BuildModeAbility buildModeAbility)
        {
            if (initialised)
            {
                return;
            }
            initialised = true;

            this.buildModeAbility = buildModeAbility;

            ownText.text = buildModeAbility.BuildModeAbilityTemplate.NameInGame;
        }

        public void UIComponentClicked()
        {
            InterfaceController.Instance.ActivateBuildModeAbility(buildModeAbility);
        }
    }
}