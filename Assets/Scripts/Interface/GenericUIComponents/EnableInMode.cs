using UnityEngine;

namespace GrimoireTD.UI
{
    public class EnableInMode : MonoBehaviour
    {
        [SerializeField]
        private GameMode enabledInMode;

        private IReadOnlyGameStateManager gameStateManager;

        public GameMode EnabledInMode
        {
            get
            {
                return enabledInMode;
            }
        }

        private void Start()
        {
            gameStateManager = GameModels.Models[0].GameStateManager;

            if (GameModels.Models[0].IsSetUp)
            {
                OnGameModelSetUp();
            }
            else
            {
                GameModels.Models[0].RegisterForOnSetUpCallback(OnGameModelSetUp);
            }
        }

        private void OnGameModelSetUp()
        {
            if (enabledInMode == GameMode.DEFEND)
            {
                if (gameStateManager.CurrentGameMode == GameMode.BUILD) SetSelfInactive();

                gameStateManager.RegisterForOnEnterBuildModeCallback(SetSelfInactive);
                gameStateManager.RegisterForOnEnterDefendModeCallback(SetSelfActive);
            }
            else
            {
                if (gameStateManager.CurrentGameMode == GameMode.DEFEND) SetSelfInactive();

                gameStateManager.RegisterForOnEnterBuildModeCallback(SetSelfActive);
                gameStateManager.RegisterForOnEnterDefendModeCallback(SetSelfInactive);
            }
        }

        private void SetSelfActive()
        {
            gameObject.SetActive(true);
        }

        private void SetSelfInactive()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (gameStateManager == null)
            {
                gameStateManager = GameModels.Models[0].GameStateManager;
            }

            if (GameModels.Models.Count > 0)
            {
                if (enabledInMode == GameMode.DEFEND)
                {
                    gameStateManager.DeregisterForOnEnterBuildModeCallback(SetSelfInactive);
                    gameStateManager.DeregisterForOnEnterDefendModeCallback(SetSelfActive);
                }
                else
                {
                    gameStateManager.DeregisterForOnEnterBuildModeCallback(SetSelfActive);
                    gameStateManager.DeregisterForOnEnterDefendModeCallback(SetSelfInactive);
                }
            }
        }
    }
}