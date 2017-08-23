using UnityEngine;
using UnityEngine.UI;
using GrimoireTD.UI;

namespace GrimoireTD.Abilities.BuildMode
{
    public class BuildModeAbilityUIComponent : MonoBehaviour
    {
        private IBuildModeAbility buildModeAbility;

        private bool initialised = false;

        [SerializeField]
        private Text ownText;

        public IBuildModeAbility BuildModeAbility
        {
            get
            {
                return buildModeAbility;
            }
        }

        public void SetUp(IBuildModeAbility buildModeAbility)
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