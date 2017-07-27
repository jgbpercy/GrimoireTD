using System;

public enum GameMode
{
    BUILD,
    DEFEND
}

public class GameStateManager : SingletonMonobehaviour<GameStateManager> {

    private GameMode gameMode = GameMode.BUILD;

    private Action OnEnterDefendModeCallback;
    private Action OnEnterBuildModeCallback;

    public GameMode CurrentGameMode
    {
        get
        {
            return gameMode;
        }
        set
        {
            CDebug.Log(CDebug.gameState, "Game State Manager received state " + value + " through interface.");
            if ( value == GameMode.DEFEND)
            {
                EnterDefendMode();
            }
            else
            {
                EnterBuildMode();
            }
        }
    }

    private void Start ()
    {
        CDebug.Log(CDebug.applicationLoading, "Game State Manager Start");
    }

    private void Update()
    {
        if ( CurrentGameMode == GameMode.DEFEND && !CreepManager.Instance.WaveIsActive && !CreepManager.Instance.TrackIdleTime )
        {
            EnterBuildMode();
        }
    }

    private bool EnterDefendMode()
    {
        CDebug.Log(CDebug.gameState, "Game State Manager EnterDefendMode called, callback member count: " + OnEnterDefendModeCallback.GetInvocationList().Length);
       
        gameMode = GameMode.DEFEND;

        CreepManager.Instance.StartNextWave();

        OnEnterDefendModeCallback?.Invoke();

        return true;
    }

    private bool EnterBuildMode()
    {
        CDebug.Log(CDebug.gameState, "Game State Manager EnterBuildMode called, callback member count: " + OnEnterBuildModeCallback.GetInvocationList().Length);

        gameMode = GameMode.BUILD;

        OnEnterBuildModeCallback?.Invoke();

        return true;
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
