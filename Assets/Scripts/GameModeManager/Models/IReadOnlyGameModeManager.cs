using System;

namespace GrimoireTD
{
    public interface IReadOnlyGameModeManager
    {
        GameMode CurrentGameMode { get; }

        event EventHandler<EAOnEnterBuildMode> OnEnterBuildMode;

        event EventHandler<EAOnEnterDefendMode> OnEnterDefendMode;
    }
}