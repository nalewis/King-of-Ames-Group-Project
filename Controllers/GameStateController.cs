using GamePieces.Session;

namespace Controllers
{
    public static class GameStateController
    {
        public static void StartTurn()
        {
            Game.StartTurn();
        }

        public static void EndTurn()
        {
            Game.EndTurn();
        }
    }
}