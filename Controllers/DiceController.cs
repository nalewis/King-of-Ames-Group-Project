using GamePieces.Dice;
using System.Collections.Generic;

namespace Controllers
{
    public static class DiceController
    {
        public static List<Die> getDice()
        {
            return GamePieces.Session.DiceRoller.Rolling;
        }

        public static void setup(int numDice)
        {
            GamePieces.Session.DiceRoller.Setup(numDice);
        }

        public static void roll()
        {
            GamePieces.Session.DiceRoller.Roll();
        }
    }
}
