using System;
using GrimoireTD.Technical;

namespace GrimoireTD.Tests
{
    public class FrameUpdaterStub : IModelObjectFrameUpdater
    {
        private Action<float> updateActions;

        private Action<float> updateActionsToAdd;
        private Action<float> updateActionsToRemove;

        private bool strictAddRemoveCycle;

        public FrameUpdaterStub()
        {
            strictAddRemoveCycle = false;
        }

        public FrameUpdaterStub(bool strictAddRemoveCycle)
        {
            this.strictAddRemoveCycle = strictAddRemoveCycle;
        }

        public void RunUpdate(float deltaTime)
        {
            updateActions?.Invoke(deltaTime);

            if (strictAddRemoveCycle)
            {
                updateActions += updateActionsToAdd;
                updateActionsToAdd = null;

                Delegate.RemoveAll(updateActions, updateActionsToRemove);
                updateActionsToRemove = null;
            }
        }

        public void Register(Action<float> updateAction)
        {
            if (strictAddRemoveCycle)
            {
                updateActionsToAdd += updateAction;
            }
            else
            {
                updateActions += updateAction;
            }
        }

        public void Deregister(Action<float> updateAction)
        {
            if (strictAddRemoveCycle)
            {
                updateActionsToRemove += updateAction;
            }
            else
            {
                updateActions -= updateAction;
            }
        }
    }
}