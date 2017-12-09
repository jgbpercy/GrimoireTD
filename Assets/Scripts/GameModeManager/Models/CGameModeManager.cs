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

    public class CGameModeManager : IGameModeManager
    {
        private GameMode _gameMode = GameMode.BUILD;

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
        
        public CGameModeManager()
        {
            DepsProv.TheInterfaceController().OnEnterDefendModePlayerAction += 
                (object sender, EAOnEnterDefendModePlayerAction args) => CurrentGameMode = GameMode.DEFEND;
        }

        public void SetUp()
        {
            DepsProv.TheCreepManager.OnWaveOver += 
                (object sender, EAOnWaveOver args) => CurrentGameMode = GameMode.BUILD;
        }
    }
}