using System;
using UnityEngine;

namespace GrimoireTD.UI
{
    public class EnableInMode : MonoBehaviour
    {
        [SerializeField]
        private GameMode enabledInMode;

        private IReadOnlyGameStateManager gameStateManager;

        private EventHandler<EAOnEnterBuildMode> OnEnterBuildMode;
        private EventHandler<EAOnEnterDefendMode> OnEnterDefendMode;

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
                GameModels.Models[0].OnGameModelSetUp += (object sender, EAOnGameModelSetUp args) => OnGameModelSetUp();
            }
        }

        private void OnGameModelSetUp()
        {
            if (enabledInMode == GameMode.DEFEND)
            {
                if (gameStateManager.CurrentGameMode == GameMode.BUILD) SetSelfInactive();

                OnEnterBuildMode = (object sender, EAOnEnterBuildMode args) => SetSelfInactive();
                OnEnterDefendMode = (object sender, EAOnEnterDefendMode args) => SetSelfActive();
            }
            else
            {
                if (gameStateManager.CurrentGameMode == GameMode.DEFEND) SetSelfInactive();

                OnEnterBuildMode = (object sender, EAOnEnterBuildMode args) => SetSelfActive();
                OnEnterDefendMode = (object sender, EAOnEnterDefendMode args) => SetSelfInactive(); 
            }

            gameStateManager.OnEnterBuildMode += OnEnterBuildMode;
            gameStateManager.OnEnterDefendMode += OnEnterDefendMode;
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
                gameStateManager.OnEnterBuildMode -= OnEnterBuildMode;
                gameStateManager.OnEnterDefendMode -= OnEnterDefendMode;
            }
        }
    }
}