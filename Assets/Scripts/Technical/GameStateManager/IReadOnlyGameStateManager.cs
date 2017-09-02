using System;

namespace GrimoireTD
{
    public interface IReadOnlyGameStateManager
    {
        GameMode CurrentGameMode { get; }

        event EventHandler<EAOnEnterBuildMode> OnEnterBuildMode;

        event EventHandler<EAOnEnterDefendMode> OnEnterDefendMode;
    }
}