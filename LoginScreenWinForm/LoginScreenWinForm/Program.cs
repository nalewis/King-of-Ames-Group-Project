using System;
using System.Windows.Forms;
using Lidgren.Network;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using System.Collections.Generic;

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
        public string hostName = "";
        public string hostIP = "";
        public static List<string> players = new List<string>(); 
        private static NetServer _server;
        private static NetClient _client;

        public Host()
        {
            Console.WriteLine("hello!");
            hostIP = GetLocalIPAddress();
            hostName = "TestHost";
            var config = new NetPeerConfiguration("King of Ames") { Port = 6969 };
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            _server = new NetServer(config);
            _server.Start();
            Console.WriteLine("Server started...");
            addServer();

            // Starts thread to handle input from clients
            Thread recieve = new Thread(recieveLoop);
            recieve.Start();

            _client = new NetClient(new NetPeerConfiguration("King of Ames"));
            _client.Start();
            var outMsg = _client.CreateMessage();
            outMsg.Write((byte)PacketTypes.Login);
            //this won't be the case for other clients
            _client.Connect("localhost", 6969, outMsg);
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

                        /*if (type == (byte)PacketTypes.NewUser)
                        {
                            Console.WriteLine("New user request.");
                            var name = inc.ReadString();
                            Console.WriteLine("Name: " + name);

                            var ip = inc.ReadString();
                            Console.WriteLine("IP: " + ip);
                            newUser(name, ip);
                        }*/
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        //Asks the webserver to create a new user with the given info
        public bool addServer()
        {
            NameValueCollection data = new NameValueCollection();
            //COMMAND is what the php looks for to determine it's actions
            data.Add("COMMAND", "addServer");
            //TODO how to pass object this far?
            data.Add("hostname", hostName);
            data.Add("hostip", hostIP);
            data.Add("players", hostName);
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var result = wc.UploadValues("http://proj-309-yt-01.cs.iastate.edu/login.php", "POST", data);
                var encresult = Encoding.ASCII.GetString(result);
                Console.WriteLine("\nResponse received was :\n{0}", encresult);
                if (encresult.Contains("INVALID"))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public void delServer()
        {
            NameValueCollection data = new NameValueCollection();
            //COMMAND is what the php looks for to determine it's actions
            data.Add("COMMAND", "delServer");
            data.Add("hostname", hostName);
            data.Add("hostip", hostIP);
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var result = wc.UploadValues("http://proj-309-yt-01.cs.iastate.edu/login.php", "POST", data);
                var encresult = Encoding.ASCII.GetString(result);
                Console.WriteLine("\nResponse received was :\n{0}", encresult);
            }
        }

        //Took from stack overflow
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
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
        public string myIP = "";
        public string myName = "";
        private static NetClient _client;
        public Client()
        {
            myIP = GetLocalIPAddress();
            myName = "TempName";

            _client = new NetClient(new NetPeerConfiguration("King of Ames"));
            _client.Start();
            var outMsg = _client.CreateMessage();
            outMsg.Write((byte)PacketTypes.Login);
            outMsg.Write(myName);
            outMsg.Write(myIP);
            //this won't be the case for other clients
            _client.Connect("localhost", 6969, outMsg);
        }

        public List<string> listServers()
        {
            NameValueCollection data = new NameValueCollection();
            //COMMAND is what the php looks for to determine it's actions
            data.Add("COMMAND", "listServers");
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var result = wc.UploadValues("http://proj-309-yt-01.cs.iastate.edu/login.php", "POST", data);
                var table = Encoding.ASCII.GetString(result);
                //Console.WriteLine("\nResponse received was :\n{0}", table);

                //Trying to put the result into a list of strings
                List<string> servers = new List<string>();
                string[] tbl = table.Split('\n');
                for(int i = 0; i<tbl.Length;i++)
                {
                    servers.Add(tbl[i]);
                }
                return servers;
            }
        }

        //Took from stack overflow
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
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
