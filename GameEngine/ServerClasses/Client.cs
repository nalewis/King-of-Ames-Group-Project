using Controllers;
using GamePieces.Dice;
using GamePieces.Monsters;
using Lidgren.Network;
using Networking;
using Networking.Actions;
using Newtonsoft.Json;
using System;
using System.Threading;
using GameEngine.GameScreens;
using GamePieces.Cards;
using System.Collections.Generic;
using System.Linq;

namespace GameEngine.ServerClasses
{
    /// <summary>
    /// Client class holds the NetClient from Lidgren
    /// </summary>
    public static class Client
    {
        public static string Conn = "";
        public static NetClient NetClient { get; } = new NetClient(new NetPeerConfiguration("King of Ames"));
        private static Thread _loop;
        private static readonly Thread GameLoop = new Thread(Program.Run);
        private static bool _shouldStop;
        public static MonsterDataPacket[] MonsterPackets;
        public static bool CanContinue = true;
        public static bool IsStart = false;
        public static List<string> MessageHistory = new List<string>();

        /// <summary>
        /// Connects the client to the server using the current ip
        /// </summary>
        /// <returns>returns true if connected, false otherwise</returns>
        public static bool Connect()
        {
            //Sends login request to Host, with player ID attached
            var outMsg = NetClient.CreateMessage();
            outMsg.Write((byte) PacketTypes.Login);
            outMsg.Write(User.PlayerId);

            //resets the receive thread
            _shouldStop = false;
            _loop = new Thread(RecieveLoop);
            _loop.Start();

            NetClient.Connect(Conn, 6969, outMsg);
            return true;
        }

        /// <summary>
        /// Main loop to recieve messages from the server
        /// </summary>
        public static void RecieveLoop()
        {
            while (!_shouldStop)
            {
                NetIncomingMessage inc;
                if ((inc = NetClient.ReadMessage()) == null) continue;
                switch (inc.MessageType)
                {
                    case NetIncomingMessageType.Error:
                        Console.WriteLine(inc.ToString());
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        Console.WriteLine("Status changed: " + inc.SenderConnection.Status);
                        if (inc.SenderConnection.Status == NetConnectionStatus.Disconnected)
                        {
                            NetClient.Shutdown("Closed");
                            //ends the receive loop
                            _shouldStop = true;
                            Conn = "";
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        var type = inc.ReadByte();
                        if (type == (byte) PacketTypes.Start)
                        {
                            var end = inc.ReadInt32();
                            MonsterPackets = new MonsterDataPacket[end];
                            for (var i = 0; i < end; i++)
                            {
                                var json = inc.ReadString();
                                MonsterPackets[i] = JsonConvert.DeserializeObject<MonsterDataPacket>(json);
                            }

                            LobbyController.StartGame(MonsterPackets);
                            //Makes this thread a STAThread, not sure if necessary...
                            GameLoop.SetApartmentState(ApartmentState.STA);
                            GameLoop.Start();
                        }
                        else if (type == (byte) PacketTypes.Update)
                        {
                            var end = inc.ReadInt32();
                            MonsterPackets = new MonsterDataPacket[end];
                            for (var i = 0; i < end; i++)
                            {
                                var json = inc.ReadString();
                                MonsterPackets[i] = JsonConvert.DeserializeObject<MonsterDataPacket>(json);
                            }

                            MonsterController.AcceptDataPackets(MonsterPackets);

                            //if (MonsterController.GetById(User.PlayerId).State == State.StartOfTurn)
                            //    MainGameScreen.SetLocalPlayerState(0);

                            if (inc.ReadByte() == (byte) PacketTypes.Dice)
                            {
                                var diceJson = inc.ReadString();
                                var dice = JsonConvert.DeserializeObject<DiceDataPacket>(diceJson);
                                DiceController.AcceptDataPacket(dice);
                            }
                            else
                            {
                                Console.Error.WriteLine("No Dice! (╯°□°）╯︵ ┻━┻");
                            }

                            if (inc.ReadByte() == (byte) PacketTypes.Cards)
                            {
                                var cardJson = inc.ReadString();
                                var cardsDataPackets = JsonConvert.DeserializeObject<CardDataPacket[]>(cardJson);
                                CardController.SetCardsForSale(cardsDataPackets.ToList()
                                    .Select(CardController.AcceptDataPacket)
                                    .ToList());
                            }
                            else
                            {
                                Console.Error.WriteLine("No Cards! (╯°□°）╯︵ ┻━┻");
                            }

                            CanContinue = true;
                        }
                        else if (type == (byte) PacketTypes.Closed)
                        {
                            NetClient.Shutdown("Closed");
                            //ends the receive loop
                            _shouldStop = true;
                            Conn = "";
                        }
                        else if (type == (byte) PacketTypes.GameOver)
                        {
                            Console.WriteLine("Game Over!");
                            var winnerName = inc.ReadString();
                            MainGameScreen.EndGame(winnerName);
                        }
                        else if (type == (byte) PacketTypes.Message)
                        {
                            var message = inc.ReadString();
                            MessageHistory.Add(message);
                        }
                        break;
                    case NetIncomingMessageType.UnconnectedData:
                        break;
                    case NetIncomingMessageType.ConnectionApproval:
                        break;
                    case NetIncomingMessageType.DebugMessage:
                        break;
                    case NetIncomingMessageType.WarningMessage:
                        break;
                    case NetIncomingMessageType.ErrorMessage:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Sends local action to server to update the game status
        /// </summary>
        /// <param name="packet"></param>
        public static void SendActionPacket(ActionPacket packet)
        {
            while (!CanContinue)
            {
                Thread.Sleep(500);
                Console.WriteLine("Sleeping packet type: " + packet.Action);
            }
            var outMsg = NetClient.CreateMessage();
            outMsg.Write((byte) PacketTypes.Action);
            var json = JsonConvert.SerializeObject(packet);
            JsonConvert.DeserializeObject<ActionPacket>(json);
            outMsg.Write(json);
            NetClient.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered);
            CanContinue = false;
        }

        //Sends a message to the server, the server will return the message to all clients, which will then add it to their message history
        public static void SendMessage(string message)
        {
            var timeStamp = DateTime.Now.ToString("mm:ss");
            message = "[" + timeStamp + "] " + message;
            var outMsg = NetClient.CreateMessage();
            outMsg.Write((byte) PacketTypes.Message);
            outMsg.Write(message);
            NetClient.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered);
        }

        public static void SendChatMessage(string message)
        {
            var outMsg = NetClient.CreateMessage();
            outMsg.Write((byte)PacketTypes.Chat);
            outMsg.Write(message);
            NetClient.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Tells the server to delete it from list, stops loop and shuts down NetClient
        /// </summary>
        public static void ClientStop()
        {
            var outMsg = NetClient.CreateMessage();
            outMsg.Write((byte) PacketTypes.Leave);
            outMsg.Write(User.PlayerId);
            NetClient.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered);
            NetClient.WaitMessage(1000);
            NetClient.Shutdown("Closed");
            //ends the receive loop
            _shouldStop = true;
            Conn = "";
            if (GameLoop.IsAlive) { GameLoop.Abort(); }
        }

        private enum PacketTypes
        {
            Login,
            Leave,
            Start,
            Action,
            Update,
            Dice,
            NoDice,
            GameOver,
            Closed,
            Chat,
            Cards,
            Message
        }
    }
}
