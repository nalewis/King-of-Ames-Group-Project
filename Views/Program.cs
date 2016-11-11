using System;
using System.Windows.Forms;
using Lidgren.Network;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using System.Collections.Generic;

//This allows us to use the classes in The Controllers - test.cs file (needed to add Controllers to References)
using Controllers.Helpers;
using Controllers.User;
using Networking;
using Controllers;

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
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
        //WIP TODO
        static void OnProcessExit(object sender, EventArgs e)
        {
            if (NetworkClasses.isHosting(User.id))
            {
                NetworkClasses.deleteServer(User.id);
            }
        }
    }

    /// <summary>
    /// Host class is where the NetServer from Lidgren is located
    /// </summary>
    static class Host
    {
        public static List<int> players = new List<int>(); 
        private static NetServer _server;

        /// <summary>
        /// Initializes the server, starts the reiceve loop, creates a NetClient and connects it to the server
        /// </summary>
        public static void serverStart()
        {
            var config = new NetPeerConfiguration("King of Ames") { Port = 6969 };
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            _server = new NetServer(config);
            _server.Start();
            Console.WriteLine("Server started...");

            //add server to the SQL database with the current details
            NetworkClasses.createServer(User.id, User.localIp);

            // Starts thread to handle input from clients
            Thread recieve = new Thread(recieveLoop);
            recieve.Start();

            //The host contains a client to behave like a user
            Client.conn = User.localIp;
            Client._client.Start();
            Client.connect();
        }

        /// <summary>
        /// Deletes the server from the database, and stop the NetServer
        /// </summary>
        public static void serverStop()
        {
            NetworkClasses.deleteServer(User.id);
            Client.clientStop();
            _server.Shutdown("Closed");
        }

        /// <summary>
        /// Main loop to recieve messages from clients
        /// </summary>
        public static void recieveLoop()
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
                            players.Add(inc.ReadInt32());

                            Console.WriteLine("Approved new connection");
                            Console.WriteLine(inc.SenderConnection.ToString() + " has connected");
                        }

                        break;
                    //The data message type encompasses all messages that aren't related to the running
                    //of the lidgren library, to differentiate, we pass different PacketTypes
                    case NetIncomingMessageType.Data:
                        //can only call readByte once, otherwise it continues reading the following bytes
                        var type = inc.ReadByte();

                        if(type == (byte)PacketTypes.leave)
                        {
                            players.Remove(inc.ReadInt32());
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        enum PacketTypes
        {
            Login,
            LoginInfo,
            NewUser,
            delUser,
            ListUsers,
            newGame,
            joinGame,
            chat,
            leave,
            close,
            Welcome,

        }

    }

    /// <summary>
    /// Client class holds the NetClient from Lidgren
    /// </summary>
    static class Client
    {
        public static string conn = "";
        public static NetClient _client = new NetClient(new NetPeerConfiguration("King of Ames"));
        private static Thread loop;
        private static bool shouldStop = false;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns true if connected, false otherwise</returns>
        public static bool connect()
        {
            //Sends login request to Host, with player ID attached
            var outMsg = _client.CreateMessage();
            outMsg.Write((byte)PacketTypes.Login);
            outMsg.Write(Int32.Parse(User.id));
            
            //resets the receive thread
            shouldStop = false;
            loop = new Thread(recieveLoop);
            loop.Start();

            _client.Connect(conn, 6969, outMsg);
           // if(_client.ConnectionStatus != NetConnectionStatus.Connected) { return false; }
            return true;
        }

        /// <summary>
        /// Main loop to recieve messages from the server
        /// </summary>
        public static void recieveLoop()
        {
            while (!shouldStop)
            {
                NetIncomingMessage inc;
                if ((inc = _client.ReadMessage()) == null) continue;
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
                        if(type == (byte)PacketTypes.Welcome)
                        {
                            Console.WriteLine(inc.ReadString());
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
        public static void clientStop()
        {
            var outMsg = _client.CreateMessage();
            outMsg.Write((byte)PacketTypes.leave);
            outMsg.Write(Int32.Parse(User.id));
            _client.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered);
            _client.WaitMessage(1000);
            _client.Shutdown("Closed");
            //ends the receive loop
            shouldStop = true;
            conn = "";
        }

        enum PacketTypes
        {
            Login,
            LoginInfo,
            NewUser,
            delUser,
            ListUsers,
            newGame,
            joinGame,
            chat,
            leave,
            close,
            Welcome,
        }
    }
}
