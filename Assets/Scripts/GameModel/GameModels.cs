using System.Collections.Generic;

namespace GrimoireTD
{
    public static class GameModels
    {
        public static List<IReadOnlyGameModel> Models { get; private set; } = new List<IReadOnlyGameModel>();
    }
}