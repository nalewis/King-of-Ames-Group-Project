using System;
using Controllers;
using GamePieces.Cards;
using GamePieces.Session;
using Newtonsoft.Json;

namespace ConsoleTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            LobbyController.AddPlayer(0, "0");
            LobbyController.AddPlayer(1, "1");
            LobbyController.StartGame();


            Console.WriteLine(CardController.CardForSaleOne().GetType());
            var packet = CardController.CreateDataPacket(CardController.CardForSaleOne());

            var json = JsonConvert.SerializeObject(packet);

            Console.WriteLine(json);
            var cardPacket = JsonConvert.DeserializeObject<CardDataPacket>(json);

            var card = CardController.AcceptDataPacket(cardPacket);

           // Console.WriteLine(card.GetType());


        }
    }
}


