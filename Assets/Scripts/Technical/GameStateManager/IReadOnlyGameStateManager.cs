using System;

namespace GrimoireTD
{
    public interface IReadOnlyGameStateManager
    {
        GameMode CurrentGameMode { get; }

        void RegisterForOnEnterBuildModeCallback(Action callback);
        void DeregisterForOnEnterBuildModeCallback(Action callback);

        void RegisterForOnEnterDefendModeCallback(Action callback);
        void DeregisterForOnEnterDefendModeCallback(Action callback);
    }
}