using System;
using UnityEngine;

namespace GrimoireTD.Technical
{
    public class ModelObjectFrameUpdater : SingletonMonobehaviour<ModelObjectFrameUpdater>, IModelObjectFrameUpdater
    {
        private Action<float> updateActions;

        private Action<float> updateActionsToAdd;
        private Action<float> updateActionsToRemove;

        private void Update()
        {
            updateActions?.Invoke(Time.deltaTime);

            updateActions += updateActionsToAdd;
            updateActionsToAdd = null;

            Delegate.RemoveAll(updateActions, updateActionsToRemove);
            updateActionsToRemove = null;
        }

        public void Register(Action<float> updateAction)
        {
            updateActionsToAdd += updateAction;
        }

        public void Deregister(Action<float> updateAction)
        {
            updateActionsToRemove += updateAction;
        }
    }
}