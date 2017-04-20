using UnityEngine;

public class EnableInMode : MonoBehaviour {

    [SerializeField]
    private GameMode enabledInMode;

	private void Start ()
    {
        if ( enabledInMode == GameMode.DEFEND )
        {
            GameStateManager.Instance.RegisterForOnEnterBuildModeCallback(() => gameObject.SetActive(false));
            GameStateManager.Instance.RegisterForOnEnterDefendModeCallback(() => gameObject.SetActive(true));
            gameObject.SetActive(false);
        }
        else
        {
            GameStateManager.Instance.RegisterForOnEnterBuildModeCallback(() => gameObject.SetActive(true));
            GameStateManager.Instance.RegisterForOnEnterDefendModeCallback(() => gameObject.SetActive(false));
        }
    }

}
