using System;
using UnityEngine;

namespace GrimoireTD.UI
{
    public class EnableInMode : MonoBehaviour
    {
        [SerializeField]
        private GameMode enabledInMode;

        private IReadOnlyGameModel gameModel;

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
            gameModel = Dependencies.DepsProv.TheGameModel;

            if (gameModel.IsSetUp)
            {
                OnGameModelSetUp();
            }
            else
            {
                gameModel.OnGameModelSetUp += (object sender, EAOnGameModelSetUp args) => OnGameModelSetUp();
            }
        }

        private void OnGameModelSetUp()
        {
            if (enabledInMode == GameMode.DEFEND)
            {
                if (gameModel.GameModeManager.CurrentGameMode == GameMode.BUILD) SetSelfInactive();

                OnEnterBuildMode = (object sender, EAOnEnterBuildMode args) => SetSelfInactive();
                OnEnterDefendMode = (object sender, EAOnEnterDefendMode args) => SetSelfActive();
            }
            else
            {
                if (gameModel.GameModeManager.CurrentGameMode == GameMode.DEFEND) SetSelfInactive();

                OnEnterBuildMode = (object sender, EAOnEnterBuildMode args) => SetSelfActive();
                OnEnterDefendMode = (object sender, EAOnEnterDefendMode args) => SetSelfInactive(); 
            }

            gameModel.GameModeManager.OnEnterBuildMode += OnEnterBuildMode;
            gameModel.GameModeManager.OnEnterDefendMode += OnEnterDefendMode;
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
            if (gameModel != null && gameModel.GameModeManager != null)
            {
                gameModel.GameModeManager.OnEnterBuildMode -= OnEnterBuildMode;
                gameModel.GameModeManager.OnEnterDefendMode -= OnEnterDefendMode;
            }
        }
    }
}