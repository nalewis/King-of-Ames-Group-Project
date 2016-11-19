using Controllers;
using GamePieces.Monsters;
using GamePieces.Session;
using Lidgren.Network;
using Networking;
using Networking.Actions;
using Newtonsoft.Json;
using System;
using System.Threading;

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
        private static Thread _gameLoop;
        private static bool _shouldStop;
        public static MonsterDataPacket[] MonsterPackets;

        /// <summary>
        /// Connects the client to the server using the current ip
        /// </summary>
        /// <returns>returns true if connected, false otherwise</returns>
        public static bool Connect()
        {
            //Sends login request to Host, with player ID attached
            var outMsg = NetClient.CreateMessage();
            outMsg.Write((byte)PacketTypes.Login);
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
                        if (type == (byte)PacketTypes.Start)
                        {
                            var end = inc.ReadInt32();
                            MonsterPackets = new MonsterDataPacket[end];
                            for (var i = 0; i < end; i++)
                            {
                                var json = inc.ReadString();
                                MonsterPackets[i] = JsonConvert.DeserializeObject<MonsterDataPacket>(json);
                            }
                            LobbyController.StartGame(MonsterPackets);
                            _gameLoop = new Thread(Program.Run);
                            _gameLoop.Start();
                        }
                        else if (type == (byte)PacketTypes.Update)
                        {
                            Console.WriteLine("Update!");
                            var end = inc.ReadInt32();
                            MonsterPackets = new MonsterDataPacket[end];
                            for (var i = 0; i < end; i++)
                            {
                                var json = inc.ReadString();
                                MonsterPackets[i] = JsonConvert.DeserializeObject<MonsterDataPacket>(json);
                            }

                            MonsterController.AcceptDataPackets(MonsterPackets);
                        }
                        else if (type == (byte)PacketTypes.Closed)
                        {
                            NetClient.Shutdown("Closed");
                            //ends the receive loop
                            _shouldStop = true;
                            Conn = "";
                        }
                        break;
                    case NetIncomingMessageType.UnconnectedData:
                        break;
                    case NetIncomingMessageType.ConnectionApproval:
                        break;
                    case NetIncomingMessageType.Receipt:
                        break;
                    case NetIncomingMessageType.DiscoveryRequest:
                        break;
                    case NetIncomingMessageType.DiscoveryResponse:
                        break;
                    case NetIncomingMessageType.VerboseDebugMessage:
                        break;
                    case NetIncomingMessageType.DebugMessage:
                        break;
                    case NetIncomingMessageType.WarningMessage:
                        break;
                    case NetIncomingMessageType.ErrorMessage:
                        break;
                    case NetIncomingMessageType.NatIntroductionSuccess:
                        break;
                    case NetIncomingMessageType.ConnectionLatencyUpdated:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static void SendActionPacket(ActionPacket packet)
        {
            var outMsg = NetClient.CreateMessage();
            outMsg.Write((byte)PacketTypes.Action);
            var json = JsonConvert.SerializeObject(packet);
            packet = JsonConvert.DeserializeObject<ActionPacket>(json);
            outMsg.Write(json);
            NetClient.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Tells the server to delete it from list, stops loop and shuts down NetClient
        /// </summary>
        public static void ClientStop()
        {
            var outMsg = NetClient.CreateMessage();
            outMsg.Write((byte)PacketTypes.Leave);
            outMsg.Write(User.PlayerId);
            NetClient.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered);
            NetClient.WaitMessage(1000);
            NetClient.Shutdown("Closed");
            //ends the receive loop
            _shouldStop = true;
            Conn = "";
        }

        private enum PacketTypes
        {
            Login,
            Leave,
            Start,
            Action,
            Update,
            Closed
        }
    }
}
