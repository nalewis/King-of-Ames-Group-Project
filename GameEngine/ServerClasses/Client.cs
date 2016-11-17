using Controllers;
using GamePieces.Monsters;
using Lidgren.Network;
using Networking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        private static bool _shouldStop;
        public static MonsterDataPacket MonsterPackets;

        /// <summary>
        /// Connects the client to the server using the current ip
        /// </summary>
        /// <returns>returns true if connected, false otherwise</returns>
        public static bool Connect()
        {
            //Sends login request to Host, with player ID attached
            var outMsg = NetClient.CreateMessage();
            outMsg.Write((byte)PacketTypes.Login);
            outMsg.Write(int.Parse(User.Id));

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
                        Console.WriteLine("Connected to Server.");
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
                            for (var i = 0; i < end; i++)
                            {
                                var json = inc.ReadString();
                                var packet = JsonConvert.DeserializeObject<MonsterDataPacket>(json);
                                if (packet.PlayerId == Int32.Parse(User.Id))
                                {
                                    Program.Run();
                                    Console.WriteLine("Packet Recieved");
                                }
                            }
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

        /// <summary>
        /// Tells the server to delete it from list, stops loop and shuts down NetClient
        /// </summary>
        public static void ClientStop()
        {
            var outMsg = NetClient.CreateMessage();
            outMsg.Write((byte)PacketTypes.Leave);
            outMsg.Write(int.Parse(User.Id));
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
            Closed
        }
    }
}
