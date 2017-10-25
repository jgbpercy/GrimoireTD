using System;
using GrimoireTD.UI;
using GrimoireTD.Creeps;
using GrimoireTD.Dependencies;

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
                    _gameMode = GameMode.DEFEND;
                    OnEnterDefendMode?.Invoke(this, new EAOnEnterDefendMode());
                }
                else
                {
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
            DepsProv.TheCreepManager.OnWaveOver += (object sender, EAOnWaveOver args) => CurrentGameMode = GameMode.BUILD;
        }

        private void SetGameModeDefend()
        {
            CurrentGameMode = GameMode.DEFEND;
        }
    }
}