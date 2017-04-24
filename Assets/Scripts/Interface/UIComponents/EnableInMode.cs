using UnityEngine;

public class EnableInMode : MonoBehaviour {

    [SerializeField]
    private GameMode enabledInMode;

	private void Start ()
    {
        if ( enabledInMode == GameMode.DEFEND )
        {
            GameStateManager.Instance.RegisterForOnEnterBuildModeCallback(SetSelfInactive);
            GameStateManager.Instance.RegisterForOnEnterDefendModeCallback(SetSelfActive);
            if ( GameStateManager.Instance.CurrentGameMode == GameMode.DEFEND )
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
            GameStateManager.Instance.RegisterForOnEnterBuildModeCallback(SetSelfActive);
            GameStateManager.Instance.RegisterForOnEnterDefendModeCallback(SetSelfInactive);
            if (GameStateManager.Instance.CurrentGameMode == GameMode.BUILD)
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
        if ( GameStateManager.InstanceNullAccepted != null )
        {
            if (enabledInMode == GameMode.DEFEND)
            {
                GameStateManager.Instance.DeregisterForOnEnterBuildModeCallback(SetSelfInactive);
                GameStateManager.Instance.DeregisterForOnEnterDefendModeCallback(SetSelfActive);
            }
            else
            {
                GameStateManager.Instance.DeregisterForOnEnterBuildModeCallback(SetSelfActive);
                GameStateManager.Instance.DeregisterForOnEnterDefendModeCallback(SetSelfInactive);
            }
        }
    }

}
