using Controllers;
using GamePieces.Session;
using Lidgren.Network;
using Networking;
using Networking.Actions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GameEngine.ServerClasses
{
    public static class Host
    {
        public static List<int> Players = new List<int>();
        private static NetServer _server;

        /// <summary>
        /// Initializes the server, starts the reiceve loop, creates a NetClient and connects it to the server
        /// </summary>
        public static void ServerStart()
        {
            var config = new NetPeerConfiguration("King of Ames") { Port = 6969 };
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            _server = new NetServer(config);
            _server.Start();
            Console.WriteLine("Server started...");

            //add server to the SQL database with the current details
            NetworkClasses.CreateServer(User.PlayerId, User.LocalIp);

            // Starts thread to handle input from clients
            var recieve = new Thread(RecieveLoop);
            recieve.Start();

            //The host contains a client to behave like a user
            Client.Conn = User.LocalIp;
            Client.NetClient.Start();
            Client.Connect();
        }

        /// <summary>
        /// Deletes the server from the database, and stop the NetServer
        /// </summary>
        public static void ServerStop()
        {
            Client.ClientStop();
            var outMsg = _server.CreateMessage();
            outMsg.Write((byte)PacketTypes.Closed);
            _server.SendToAll(outMsg, NetDeliveryMethod.ReliableOrdered);
            _server.Shutdown("Closed");
            NetworkClasses.DeleteServer(User.PlayerId);
        }

        /// <summary>
        /// Main loop to recieve messages from clients
        /// </summary>
        public static void RecieveLoop()
        {
            while (true)
            {
                NetIncomingMessage inc;
                if ((inc = _server.ReadMessage()) == null) continue;
                switch (inc.MessageType)
                {
                    case NetIncomingMessageType.Error:
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        Console.WriteLine("Client status changed: " + inc.SenderConnection.Status);
                        break;
                    case NetIncomingMessageType.ConnectionApproval:
                        //Initially approves connecting clients based on their login byte
                        if (inc.ReadByte() == (byte)PacketTypes.Login)
                        {
                            Console.WriteLine(inc.MessageType);

                            inc.SenderConnection.Approve();
                            Players.Add(inc.ReadInt32());
                            if (Players.Count == 6) { NetworkClasses.UpdateServerStatus("Starting", User.PlayerId); }

                            Console.WriteLine("Approved new connection");
                            Console.WriteLine(inc.SenderConnection + " has connected");
                        }

                        break;
                    //The data message type encompasses all messages that aren't related to the running
                    //of the lidgren library, to differentiate, we pass different PacketTypes
                    case NetIncomingMessageType.Data:
                        //can only call readByte once, otherwise it continues reading the following bytes
                        var type = inc.ReadByte();

                        if (type == (byte)PacketTypes.Leave)
                        {
                            Players.Remove(inc.ReadInt32());
                        }
                        else if(type == (byte)PacketTypes.Action)
                        {
                            var json = inc.ReadString();
                            ActionPacket packet = JsonConvert.DeserializeObject<ActionPacket>(json);
                            ReceiveActionUpdate(packet);
                        }
                        break;
                    case NetIncomingMessageType.UnconnectedData:
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
        /// Sends the monter packets to all connected users
        /// </summary>
        public static void StartGame()
        {
            Game.StartTurn();
            //MonsterController.AcceptDataPackets(MonsterController.GetDataPackets());
            SendMonsterPackets(true);
        }

        public static void ReceiveActionUpdate(ActionPacket packet)
        {
            GameStateController.AcceptAction(packet);

            SendMonsterPackets(false);
        }

        public static void SendMonsterPackets(bool start)
        {
            var outMsg = _server.CreateMessage();
            if (start) { outMsg.Write((byte)PacketTypes.Start);}
            else { outMsg.Write((byte)PacketTypes.Update); }
            outMsg.Write(Players.Count);
            var packets = MonsterController.GetDataPackets();
            for (var i = 0; i < Players.Count; i++)
            {
                var json = JsonConvert.SerializeObject(packets[i]);
                outMsg.Write(json);
            }
            _server.SendToAll(outMsg, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Gets the ping values for all connected users
        /// </summary>
        /// <returns>Int list</returns>
        public static List<int> GetPing()
        {
            return _server.Connections.Select(conn => (int)(conn.AverageRoundtripTime * 1000)).ToList();
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
