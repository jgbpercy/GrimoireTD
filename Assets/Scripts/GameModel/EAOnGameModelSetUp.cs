using System;

namespace GrimoireTD
{
    public class EAOnGameModelSetUp : EventArgs
    {
        public readonly IReadOnlyGameModel GameModel;

        public EAOnGameModelSetUp(IReadOnlyGameModel gameModel)
        {
            GameModel = gameModel;
        }
    }
}