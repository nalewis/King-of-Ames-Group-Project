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

    class Host
    {
        public static List<string> players = new List<string>(); 
        private static NetServer _server;
        private static NetClient _client;


        public Host()
        {
            var config = new NetPeerConfiguration("King of Ames"){Port = 6969};
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
            _client = new NetClient(new NetPeerConfiguration("King of Ames"));
            _client.Start();
            var outMsg = _client.CreateMessage();
            outMsg.Write((byte)PacketTypes.Login);
            _client.Connect(User.localIp, 6969, outMsg);
        }

        public void serverStop()
        {
            NetworkClasses.deleteServer(User.id);
            _server.Shutdown("Closed");
        }

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
                            players.Add(inc.SenderConnection.ToString());

                            NetOutgoingMessage outMsg = _server.CreateMessage();
                            outMsg.Write((byte)PacketTypes.Welcome);
                            outMsg.Write("WELCOME TO SERVER");
                            _server.SendMessage(outMsg, inc.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);

                            Console.WriteLine("Approved new connection");
                            Console.WriteLine(inc.SenderConnection.ToString() + " has connected");
                        }

                        break;
                    //The data message type encompasses all messages that aren't related to the running
                    //of the lidgren library, to differentiate, we pass different PacketTypes
                    case NetIncomingMessageType.Data:
                        //can only call readByte once, otherwise it continues reading the following bytes
                        var type = inc.ReadByte();

                        if (type == (byte)PacketTypes.ListUsers)
                        {
                            listUsers();
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static void listUsers()
        {
            NameValueCollection data = new NameValueCollection();
            //COMMAND is what the php looks for to determine it's actions
            data.Add("COMMAND", "listUsers");
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var result = wc.UploadValues("http://proj-309-yt-01.cs.iastate.edu/login.php", "POST", data);
                var encresult = Encoding.ASCII.GetString(result);
                Console.WriteLine("\nResponse received was :\n{0}", encresult);
                NetOutgoingMessage outMsg = _client.CreateMessage();
                outMsg.Write((byte)PacketTypes.ListUsers);
                string[] tbl = encresult.Split('\n');

                for (int i = 0; i < tbl.Length; i++)
                {
                    outMsg.Write(tbl[i]);
                }
                _server.SendToAll(outMsg, NetDeliveryMethod.ReliableOrdered);
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

    static class Client
    {
        public static string myIP = User.localIp;
        public static string myName = User.username;
        public static bool isConnecting = false;
        public static string conn = "";
        public static NetClient _client = new NetClient(new NetPeerConfiguration("King of Ames"));
        private static Thread loop;
        private static bool shouldStop = false;


        public static bool connect()
        {
            _client.Start();
            var outMsg = _client.CreateMessage();
            outMsg.Write((byte)PacketTypes.Login);
            
            //resets the receive thread
            shouldStop = false;
            loop = new Thread(recieveLoop);
            loop.Start();

            _client.Connect(conn, 6969, outMsg);
            return true;
        }

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
                        /*else if(type==(byte)PacketTypes.ListUsers)
                        {
                            while(inc.PeekString() != null)
                            {
                                others.Add(inc.ReadString());
                            }
                            Console.WriteLine("Received Users");
                        }*/
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static void clientStop()
        {
            //reset variables to make sure they're up to date
            conn = "";
            myIP = User.localIp;
            myName = User.username;

            _client.Shutdown("Closed");

            //ends the receive loop
            shouldStop = true;
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
