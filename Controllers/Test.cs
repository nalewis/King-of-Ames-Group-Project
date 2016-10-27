using System.Collections.Generic;
using GamePieces.Dice;
using GamePieces.Session;
using System.Web.Script.Serialization;
using System.Net;
using System.Net.Sockets;
using System;

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

    public static class Helpers
    {
        public static string ToJSON(this object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        //Took from stack overflow
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
    }

}