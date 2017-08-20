using System.Collections.Generic;

namespace GrimoireTD
{
    public static class GameModels
    {
        public static List<IGameModel> Models { get; private set; } = new List<IGameModel>();
    }
}