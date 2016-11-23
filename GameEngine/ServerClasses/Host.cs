using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Controllers;
using GamePieces.Session;
using Lidgren.Network;
using Networking;
using Networking.Actions;
using Newtonsoft.Json;

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
                        if (inc.SenderConnection.Status == NetConnectionStatus.Disconnected)
                        {
                            Console.WriteLine("Client " + inc.SenderConnection.ToString() + " status changed: " + inc.SenderConnection.Status);
                        }
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
                            if (Players.Count == 6) { NetworkClasses.UpdateServerStatus("Creating", User.PlayerId); }
                            Players.Remove(inc.ReadInt32());
                        }
                        else if(type == (byte)PacketTypes.Action)
                        {
                            var json = inc.ReadString();
                            var packet = JsonConvert.DeserializeObject<ActionPacket>(json);
                            ReceiveActionUpdate(packet);
                        }
                        else if (type == (byte) PacketTypes.Chat)
                        {
                            
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
            SendMonsterPackets(true);
        }

        /// <summary>
        /// Takes in action from client, updates the rest of the clients
        /// </summary>
        /// <param name="packet"></param>
        public static void ReceiveActionUpdate(ActionPacket packet)
        {
            //Console.WriteLine("Packet: " + packet.Action);
            //Console.WriteLine("Before: " + Game.Current.State + " id: " + Game.Current.PlayerId);
            GameStateController.AcceptAction(packet);
            //Console.WriteLine("After: " + Game.Current.State + " id: " + Game.Current.PlayerId);
            if (GameStateController.GameOver)
            {
                DeclareWinner();
            }
            else
            {
                SendMonsterPackets(sendDice: packet.Action == Networking.Actions.Action.Roll || packet.Action == Networking.Actions.Action.EndRolling);
            }
        }

        /// <summary>
        /// Sends packets to clients 
        /// </summary>
        /// <param name="start"></param>
        public static void SendMonsterPackets(bool start = false, bool sendDice = false)
        {
            var outMsg = _server.CreateMessage();
            if (start) { outMsg.Write((byte)PacketTypes.Start);}
            else { outMsg.Write((byte)PacketTypes.Update); }
            var packets = MonsterController.GetDataPackets();
            outMsg.Write(packets.Length);
            foreach (var packet in packets)
            {
                var json = JsonConvert.SerializeObject(packet);//TODO check player count
                outMsg.Write(json);
            }

            if (sendDice)
            {
                outMsg.Write((byte)PacketTypes.Dice);
                var dice = DiceController.GetDataPacket();
                outMsg.Write(JsonConvert.SerializeObject(dice));
            }
            else if(!start)
            {
                outMsg.Write((byte)PacketTypes.NoDice);
            }

            _server.SendToAll(outMsg, NetDeliveryMethod.ReliableOrdered);
        }

        public static void DeclareWinner()
        {
            //send packet type game over, update player stats, show final scores
            var outMsg = _server.CreateMessage();
            outMsg.Write((byte)PacketTypes.GameOver);
            outMsg.Write(Game.Winner.Name);
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
            Dice,
            NoDice,
            GameOver,
            Closed,
            Chat
        }

    }
}
