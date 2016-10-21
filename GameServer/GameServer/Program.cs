using System;
using Lidgren.Network;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Collections.Specialized;

public struct Lobby
{
    private String hostIP;
    private String hostName;
    private String lobbyName;

    public Lobby(String ip, String name, String lname)
    {
        hostIP = ip;
        hostName = name;
        lobbyName = lname;
    }
    public void getIP()
    {

    }
}

namespace GameServer
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        private static NetServer _server;


        [STAThread]
        public static void Main()
        {
            //            using (var game = new Game1())
            //                game.Run();

            //starts server and enables connections
            var config = new NetPeerConfiguration("King of Ames") { Port = 6969 };
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            _server = new NetServer(config);
            _server.Start();
            Console.WriteLine("Server started...");

            //At the moment it just responds to input
            //TODO implement threading to return messages
            Thread recieve = new Thread(recieveLoop);
            recieve.Start();

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
                    case NetIncomingMessageType.UnconnectedData:
                        Console.WriteLine("Hey");
                        break;
                    case NetIncomingMessageType.ConnectionApproval:
                        //Initially approves connecting clients based on their login byte
                        if (inc.ReadByte() == (byte)PacketTypes.Login)
                        {
                            Console.WriteLine(inc.MessageType);

                            inc.SenderConnection.Approve();

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

                        if (type == (byte)PacketTypes.NewUser)
                        {
                            Console.WriteLine("New user request.");
                            var name = inc.ReadString();
                            Console.WriteLine("Name: " + name);

                            var ip = inc.ReadString();
                            Console.WriteLine("IP: " + ip);
                            newUser(name, ip);
                        }
                        else if (type == (byte)PacketTypes.delUser)
                        {
                            Console.WriteLine("Delete user request.");
                            var name = inc.ReadString();
                            Console.WriteLine("Name: " + name);

                            var ip = inc.ReadString();
                            Console.WriteLine("IP: " + ip);
                            delUser(name, ip);
                        }
                        else if (type == (byte)PacketTypes.ListUsers)
                        {
                            Console.WriteLine("List users request.");
                            //Testing threads
                            Thread listing = new Thread(() => listUsers(inc));
                            listing.Start();
                            //listUsers();
                        }
                        else if (type == (byte)PacketTypes.newGame)
                        {
                            var ip = inc.ReadString();
                            Console.WriteLine(ip + " has opened a lobby.");
                            Thread game = new Thread(() => newGame(ip));
                            game.Start();
                        }
                        else if (type == (byte)PacketTypes.joinGame)
                        {

                        }
                        else if (type == (byte)PacketTypes.close)
                        {
                            var ip = inc.ReadString();
                            Console.WriteLine(ip + " has closed a lobby.");
                            break;
                        }
                        else if (type == (byte)PacketTypes.chat)
                        {
                            var message = inc.ReadString();
                            Console.WriteLine(inc.SenderConnection.ToString() + ":" + message);
                            NetOutgoingMessage outMsg = _server.CreateMessage();
                            outMsg.Write((byte)PacketTypes.chat);
                            outMsg.Write(message);
                            _server.SendMessage(outMsg, inc.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                        }
                        else if (type == (byte)PacketTypes.leave)
                        {

                        }
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

        //Asks the webserver to create a new user with the given info
        public static void newUser(string name, string ip)
        {
            NameValueCollection data = new NameValueCollection();
            //COMMAND is what the php looks for to determine it's actions
            data.Add("COMMAND", "NewPlayer");
            data.Add("name", name);
            data.Add("ip", ip);
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var result = wc.UploadValues("http://proj-309-yt-01.cs.iastate.edu/login.php", "POST", data);
                Console.WriteLine("\nResponse received was :\n{0}", Encoding.ASCII.GetString(result));
            }
        }

        //Asks the webserver to delete a user with matching name and ip
        public static void delUser(string name, string ip)
        {
            NameValueCollection data = new NameValueCollection();
            //COMMAND is what the php looks for to determine it's actions
            data.Add("COMMAND", "DelPlayer");
            data.Add("name", name);
            data.Add("ip", ip);
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var result = wc.UploadValues("http://proj-309-yt-01.cs.iastate.edu/login.php", "POST", data);
                Console.WriteLine("\nResponse received was :\n{0}", Encoding.ASCII.GetString(result));
            }
        }

        public static void listUsers(NetIncomingMessage inc)
        {
            NameValueCollection data = new NameValueCollection();
            //COMMAND is what the php looks for to determine it's actions
            data.Add("COMMAND", "ListPlayers");
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var result = wc.UploadValues("http://proj-309-yt-01.cs.iastate.edu/login.php", "POST", data);
                var table = Encoding.ASCII.GetString(result);
                Console.WriteLine("\nResponse received was :\n{0}", table);
                NetOutgoingMessage sendMsg = _server.CreateMessage();
                sendMsg.Write((byte)PacketTypes.ListUsers);
                sendMsg.Write(table);
                _server.SendMessage(sendMsg, inc.SenderConnection, NetDeliveryMethod.ReliableOrdered);
            }

        }

        public static void newGame(String ip)
        {
            Thread gameLoop = new Thread(recieveLoop);
            gameLoop.Start();
        }

        //Old function originally used to try and talk to web server
        /*public static string webTalk()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://proj-309-yt-01.cs.iastate.edu/sampleDB.php");
            request.Method = "POST";

            var dict = new Dictionary<string, string>();
            dict.Add("Nick", "Is handsome!");
            var str = "&message[]=";
            str += string.Join(Environment.NewLine, dict);
            ASCIIEncoding encoding = new ASCIIEncoding();
            
            byte[] data = encoding.GetBytes(str);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            var responseStream = new StreamReader(response.GetResponseStream());
            var messageIn = responseStream.ReadToEnd();
            Console.WriteLine("Message: " + messageIn);

            return messageIn;
        }*/
    }

    //List of different message types for the game
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