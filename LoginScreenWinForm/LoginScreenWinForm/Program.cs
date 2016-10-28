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
using Controllers.test;
using Controllers.User;

namespace LoginScreenWinForm
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the appliction.
        /// </summary>a
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }

    class LoginStuff
    {

        public static void handleUserInput(string user, string pass, Form form, Label resultLabel)
        {
            //Console.WriteLine(user + " : " + pass);
            LoginController control = new LoginController(user, pass);
            if (control.login())
            {
                Form mainMenu = new MainMenuForm();
                mainMenu.Show();
                form.Hide();
            }
            else
            {
                resultLabel.Show();
            }
            //This method is called when clicking the button so I suppose login logic goes here? not really sure though.
        }

    }

    class NewUser
    {
        public static void handleUserInput(string user, string pass, Label resultLabel)
        {
            resultLabel.Show();
            NewUserController control = new NewUserController(user, pass);
            control.createUser();
            //This method is called when clicking the button so I suppose login logic goes here? not really sure though.
        }
    }

    class Host
    {
        public string hostName = User.username;
        public string hostIP = User.localIp;
        public static List<string> players = new List<string>(); 
        private static NetServer _server;
        private static NetClient _client;

        PlayerDetails playerDetails;

        public Host()
        {
            //initialize player details object to be placed in server list array
            playerDetails = new PlayerDetails();
            playerDetails.name = hostName;
            playerDetails.character = "";
            playerDetails.ip = hostIP;


            var config = new NetPeerConfiguration("King of Ames"){Port = 6969};
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            _server = new NetServer(config);
            _server.Start();
            Console.WriteLine("Server started...");

            //add server to the SQL database with the current details
            addServer();

            // Starts thread to handle input from clients
            Thread recieve = new Thread(recieveLoop);
            recieve.Start();

            //The host contains a client to behave like a user
            _client = new NetClient(new NetPeerConfiguration("King of Ames"));
            _client.Start();
            var outMsg = _client.CreateMessage();
            outMsg.Write((byte)PacketTypes.Login);
            _client.Connect(hostIP, 6969, outMsg);
        }

        public void serverStop()
        {
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

        //Asks the webserver to create a new user with the given info
        public bool addServer()
        {
            NameValueCollection data = new NameValueCollection();
            //COMMAND is what the php looks for to determine it's actions
            data.Add("COMMAND", "addServer");
            data.Add("playerDetails", Helpers.ToJSON(playerDetails));
            data.Add("hostname", hostName);
            data.Add("hostip", hostIP);

            var response = Helpers.WebMessage(data);
            if (response.Contains("INVALID"))
            {
                return false;
            } else
            {
                return true;
            }


        }

        public void delServer()
        {
            NameValueCollection data = new NameValueCollection();
            //COMMAND is what the php looks for to determine it's actions
            data.Add("COMMAND", "delServer");
            data.Add("hostname", hostName);
            data.Add("hostip", hostIP);

            var response = Helpers.WebMessage(data);
            Console.WriteLine("\nResponse received was :\n{0}", response);
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
    class Client
    {
        public string myIP = User.localIp;
        public string myName = User.username;
        public bool isConnecting = false;
        public string conn = "";
        private static NetClient _client;
        private List<string> others = new List<string>();

        PlayerDetails playerDetails;

        public Client()
        {
            playerDetails = new PlayerDetails();
            playerDetails.name = myName;
            playerDetails.ip = myIP;

            _client = new NetClient(new NetPeerConfiguration("King of Ames"));
            _client.Start();
        }

        public bool connect()
        {
            var outMsg = _client.CreateMessage();
            outMsg.Write((byte)PacketTypes.Login);
            Thread loop = new Thread(recieveLoop);
            loop.Start();
            _client.Connect(conn, 6969, outMsg);
          //  if(_client.ConnectionStatus == NetConnectionStatus.Disconnected) { return false; }
            return true;
        }

        public void recieveLoop()
        {
            while (true)
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
                        else if(type==(byte)PacketTypes.ListUsers)
                        {
                            while(inc.PeekString() != null)
                            {
                                others.Add(inc.ReadString());
                            }
                            Console.WriteLine("Recieved Users");
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public ServerDetails[] listServers()
        {
            NameValueCollection data = new NameValueCollection();
            //COMMAND is what the php looks for to determine it's actions
            data.Add("COMMAND", "listServers");
            var servers = Helpers.FromJSON(Helpers.WebMessage(data));
            return servers;
        }

        public void joinServer(string hostname, string hostIP)
        {
            NameValueCollection data = new NameValueCollection();
            data.Add("COMMAND", "updateServer");
            //will update sql with player data
            data.Add("ACTION", "addPlayer");
            data.Add("playerDetails", Helpers.ToJSON(playerDetails));
            data.Add("hostIP", hostIP);
            data.Add("hostname", hostname);

            var response = Helpers.WebMessage(data);
            Console.WriteLine("\nResponse received was :\n{0}", response);
        }

        public List<string> getOthers()
        {
            return others;
        }

        public void clientStop()
        {
            _client.Shutdown("Closed");
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
