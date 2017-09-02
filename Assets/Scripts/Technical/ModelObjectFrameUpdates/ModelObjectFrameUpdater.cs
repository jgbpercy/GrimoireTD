using System.Collections.Generic;
using GrimoireTD.ChannelDebug;

namespace GrimoireTD.Technical
{
    public class ModelObjectFrameUpdater : SingletonMonobehaviour<ModelObjectFrameUpdater>
    {
        private List<IFrameUpdatee> modelObjectFrameUpdatees;

        private List<IFrameUpdatee> frameUpdateesToAdd;
        private List<IFrameUpdatee> frameUpdateesToRemove;

        private void Awake()
        {
            modelObjectFrameUpdatees = new List<IFrameUpdatee>();
            frameUpdateesToAdd = new List<IFrameUpdatee>();
            frameUpdateesToRemove = new List<IFrameUpdatee>();
        }

        private void Start()
        {
            CDebug.Log(CDebug.applicationLoading, "Model Object Frame Updater Start");
        }

        private void Update()
        {
            foreach (var modelObjectFrameUpdatee in modelObjectFrameUpdatees)
            {
                modelObjectFrameUpdatee.ModelObjectFrameUpdate();
            }

            foreach (var updateeToAdd in frameUpdateesToAdd)
            {
                modelObjectFrameUpdatees.Add(updateeToAdd);
            }

            frameUpdateesToAdd.Clear();

            foreach (var updateeToRemove in frameUpdateesToRemove)
            {
                modelObjectFrameUpdatees.RemoveAll(x => x == updateeToRemove);
            }

            frameUpdateesToRemove.Clear();
        }

        public void RegisterAsModelObjectFrameUpdatee(IFrameUpdatee modelObjectFrameUpdatee)
        {
            frameUpdateesToAdd.Add(modelObjectFrameUpdatee);
        }

        public void DeregisterAsModelObjectFrameUpdatee(IFrameUpdatee modelObjectFrameUpdatee)
        {
            frameUpdateesToRemove.Add(modelObjectFrameUpdatee);
        }
    }
}