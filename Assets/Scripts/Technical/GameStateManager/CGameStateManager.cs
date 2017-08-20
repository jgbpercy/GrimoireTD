using System;
using GrimoireTD.Creeps;
using GrimoireTD.ChannelDebug;
using GrimoireTD.UI;

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

        private Action OnEnterDefendModeCallback;
        private Action OnEnterBuildModeCallback;

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
                    CDebug.Log(CDebug.gameState, "Game State Manager Entered Defend Mode, callback member count: " + OnEnterDefendModeCallback.GetInvocationList().Length);

                    _gameMode = GameMode.DEFEND;

                    OnEnterDefendModeCallback?.Invoke();
                }
                else
                {
                    CDebug.Log(CDebug.gameState, "Game State Manager Entered Build Mode, callback member count: " + OnEnterBuildModeCallback.GetInvocationList().Length);

                    _gameMode = GameMode.BUILD;

                    OnEnterBuildModeCallback?.Invoke();
                }
            }
        }

        public CGameStateManager()
        {
            CurrentGameMode = GameMode.BUILD;

            InterfaceController.Instance.RegisterForOnEnterDefendModeUserAction(SetGameModeDefend);
        }

        public void SetUp()
        {
            GameModels.Models[0].CreepManager.RegisterForOnWaveIsOverCallback(() => CurrentGameMode = GameMode.BUILD);
        }

        private void SetGameModeDefend()
        {
            CurrentGameMode = GameMode.DEFEND;
        }

        public void RegisterForOnEnterDefendModeCallback(Action callback)
        {
            OnEnterDefendModeCallback += callback;
        }

        public void DeregisterForOnEnterDefendModeCallback(Action callback)
        {
            OnEnterDefendModeCallback -= callback;
        }

        public void RegisterForOnEnterBuildModeCallback(Action callback)
        {
            OnEnterBuildModeCallback += callback;
        }

        public void DeregisterForOnEnterBuildModeCallback(Action callback)
        {
            OnEnterBuildModeCallback -= callback;
        }
    }
}