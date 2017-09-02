using System;
using GrimoireTD.ChannelDebug;
using GrimoireTD.UI;
using GrimoireTD.Creeps;

namespace GrimoireTD
{
    public enum GameMode
    {
        BUILD,
        DEFEND
    }

    public class CGameStateManager : IGameStateManager
    {
        private GameMode _gameMode;

        public event EventHandler<EAOnEnterBuildMode> OnEnterBuildMode;
        public event EventHandler<EAOnEnterDefendMode> OnEnterDefendMode;

        public GameMode CurrentGameMode
        {
            get
            {
                return _gameMode;
            }
            private set
            {
                if (value == GameMode.DEFEND)
                {
                    CDebug.Log(CDebug.gameState, "Game State Manager Entered Defend Mode, callback member count: " + OnEnterDefendMode?.GetInvocationList().Length);

                    _gameMode = GameMode.DEFEND;

                    OnEnterDefendMode?.Invoke(this, new EAOnEnterDefendMode());
                }
                else
                {
                    CDebug.Log(CDebug.gameState, "Game State Manager Entered Build Mode, callback member count: " + OnEnterBuildMode?.GetInvocationList().Length);

                    _gameMode = GameMode.BUILD;

                    OnEnterBuildMode?.Invoke(this, new EAOnEnterBuildMode());
                }
            }
        }

        public CGameStateManager()
        {
            CurrentGameMode = GameMode.BUILD;

            InterfaceController.Instance.OnEnterDefendModePlayerAction += (object sender, EAOnEnterDefendModePlayerAction args) => SetGameModeDefend();
        }

        public void SetUp()
        {
            GameModels.Models[0].CreepManager.OnWaveOver += (object sender, EAOnWaveOver args) => CurrentGameMode = GameMode.BUILD;
        }

        private void SetGameModeDefend()
        {
            CurrentGameMode = GameMode.DEFEND;
        }
    }
}