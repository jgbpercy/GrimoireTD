using System.Collections.Generic;

public class ModelObjectFrameUpdater : SingletonMonobehaviour<ModelObjectFrameUpdater> {

    private List<IFrameUpdatee> modelObjectFrameUpdatees;

	private void Awake ()
    {
        modelObjectFrameUpdatees = new List<IFrameUpdatee>();
	}

    private void Start()
    {
        CDebug.Log(CDebug.applicationLoading, "Model Object Frame Updater Start");
    }

    private void Update ()
    {
        foreach (IFrameUpdatee modelObjectFrameUpdatee in modelObjectFrameUpdatees)
        {
            modelObjectFrameUpdatee.ModelObjectFrameUpdate();
        }
	}

    public void RegisterAsModelObjectFrameUpdatee(IFrameUpdatee modelObjectFrameUpdatee)
    {
        modelObjectFrameUpdatees.Add(modelObjectFrameUpdatee);
    }

    public void DeregisterAsModelObjectFrameUpdatee(IFrameUpdatee modelObjectFrameUpdatee)
    {
        //modelObjectFrameUpdatees.Remove(modelObjectFrameUpdatee);

        modelObjectFrameUpdatees.RemoveAll(x => x == modelObjectFrameUpdatee);
    }

}
