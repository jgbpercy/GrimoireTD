using UnityEngine;

namespace GrimoireTD.UI
{
    public class EnableInMode : MonoBehaviour
    {
        [SerializeField]
        private GameMode enabledInMode;

        public GameMode EnabledInMode
        {
            get
            {
                return enabledInMode;
            }
        }

        private void Start()
        {
            var gameStateManager = GameModels.Models[0].GameStateManager;

            if (enabledInMode == GameMode.DEFEND)
            {
                gameStateManager.RegisterForOnEnterBuildModeCallback(SetSelfInactive);
                gameStateManager.RegisterForOnEnterDefendModeCallback(SetSelfActive);
                if (gameStateManager.CurrentGameMode == GameMode.DEFEND)
                {
                    gameObject.SetActive(true);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                gameStateManager.RegisterForOnEnterBuildModeCallback(SetSelfActive);
                gameStateManager.RegisterForOnEnterDefendModeCallback(SetSelfInactive);
                if (gameStateManager.CurrentGameMode == GameMode.BUILD)
                {
                    gameObject.SetActive(true);
                }
                else
                {
                    gameObject.SetActive(false);
                }

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
            if (GameModels.Models.Count > 0)
            {
                var gameStateManager = GameModels.Models[0].GameStateManager;
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