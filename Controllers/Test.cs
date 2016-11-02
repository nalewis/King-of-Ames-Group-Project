using System.Collections.Generic;
using GamePieces.Dice;
using GamePieces.Session;
using System.Web.Script.Serialization;
using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Specialized;
using System.Text;

namespace Controllers.test
{
    public static class Test
    {
        public static void StartGame(List<string> names)
        {
            Game.StartGame(names);
        }

        public static List<Die> GetDice()
        {
            return DiceRoller.Rolling;
        }
    }
}