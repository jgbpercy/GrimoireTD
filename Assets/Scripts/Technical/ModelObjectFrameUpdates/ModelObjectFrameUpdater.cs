using System.Collections.Generic;
using UnityEngine;

namespace GrimoireTD.Technical
{
    public class ModelObjectFrameUpdater : SingletonMonobehaviour<ModelObjectFrameUpdater>
    {
        private List<IFrameUpdatee> modelObjectFrameUpdatees = new List<IFrameUpdatee>();

        private List<IFrameUpdatee> frameUpdateesToAdd = new List<IFrameUpdatee>();
        private List<IFrameUpdatee> frameUpdateesToRemove = new List<IFrameUpdatee>();

        private void Update()
        {
            foreach (var modelObjectFrameUpdatee in modelObjectFrameUpdatees)
            {
                modelObjectFrameUpdatee.ModelObjectFrameUpdate(Time.deltaTime);
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