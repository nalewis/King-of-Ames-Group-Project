using System;
using System.Windows.Forms;
using Lidgren.Network;
using System.Threading;
using System.Collections.Generic;
using Controllers;
using GamePieces.Monsters;

//This allows us to use the classes in The Controllers - test.cs file (needed to add Controllers to References)
using Networking;
using Newtonsoft.Json;

namespace Views
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the appliction.
        /// </summary>a
        [STAThread]
        static void Main()
        {
            //Handler for deleting a server entry if the user suddenly quits
            //AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form form = new LoginForm();
            form.Show();
            Application.Run();
        }
    }

    /// <summary>
    /// Host class is where the NetServer from Lidgren is located
    /// </summary>
    static class Host
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
            NetworkClasses.CreateServer(User.Id, User.LocalIp);

            // Starts thread to handle input from clients
            Thread recieve = new Thread(RecieveLoop);
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
            NetworkClasses.DeleteServer(User.Id);
            Client.ClientStop();
            _server.Shutdown("Closed");
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

                            Console.WriteLine("Approved new connection");
                            Console.WriteLine(inc.SenderConnection + " has connected");
                        }

                        break;
                    //The data message type encompasses all messages that aren't related to the running
                    //of the lidgren library, to differentiate, we pass different PacketTypes
                    case NetIncomingMessageType.Data:
                        //can only call readByte once, otherwise it continues reading the following bytes
                        var type = inc.ReadByte();

                        if(type == (byte)PacketTypes.Leave)
                        {
                            Players.Remove(inc.ReadInt32());
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static void StartGame()
        {
            var outMsg = _server.CreateMessage();
            outMsg.Write((byte)PacketTypes.Start);
            outMsg.Write((Int32)Players.Count);
            var packets = MonsterController.GetDataPackets();
            for (int i = 0; i < Players.Count; i++)
            {
                var json = JsonConvert.SerializeObject(packets[i]);
                outMsg.Write(json);
            }
            _server.SendToAll(outMsg,NetDeliveryMethod.ReliableOrdered);
        }

        public static List<int> GetPing()
        {
            List<int> pings = new List<int>();
            foreach(NetConnection conn in _server.Connections)
            {
                pings.Add((int)(conn.AverageRoundtripTime*1000));
            }
            return pings;
        }

        enum PacketTypes
        {
            Login,
            Leave,
            Start
        }

    }

    /// <summary>
    /// Client class holds the NetClient from Lidgren
    /// </summary>
    static class Client
    {
        public static string Conn = "";
        public static NetClient NetClient { get; } = new NetClient(new NetPeerConfiguration("King of Ames"));
        private static Thread _loop;
        private static bool _shouldStop;

        /// <summary>
        /// Connects the client to the server using the current ip
        /// </summary>
        /// <returns>returns true if connected, false otherwise</returns>
        public static bool Connect()
        {
            //Sends login request to Host, with player ID attached
            var outMsg = NetClient.CreateMessage();
            outMsg.Write((byte)PacketTypes.Login);
            outMsg.Write(Int32.Parse(User.Id));
            
            //resets the receive thread
            _shouldStop = false;
            _loop = new Thread(RecieveLoop);
            _loop.Start();

            NetClient.Connect(Conn, 6969, outMsg);
           // if(NetClient.ConnectionStatus != NetConnectionStatus.Connected) { return false; }
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
                        break;
                    case NetIncomingMessageType.Data:
                        var type = inc.ReadByte(); 
                        if(type == (byte)PacketTypes.Start)
                        {
                            for (int i = 0; i < inc.ReadInt32(); i++)
                            {
                                var packet = JsonConvert.DeserializeObject<MonsterDataPacket>(inc.ReadString());
                                if (packet.PlayerId == Int32.Parse(User.Id)) Console.WriteLine("Fuckin");
                            }
                        }
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
            outMsg.Write(Int32.Parse(User.Id));
            NetClient.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered);
            NetClient.WaitMessage(1000);
            NetClient.Shutdown("Closed");
            //ends the receive loop
            _shouldStop = true;
            Conn = "";
        }

        enum PacketTypes
        {
            Login,
            Leave,
            Start,
        }
    }
}
