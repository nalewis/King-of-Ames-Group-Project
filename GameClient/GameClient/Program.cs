using System;
using System.Net;
using System.Diagnostics;
using System.Threading;
using Lidgren.Network;
using System.Net.Sockets;

namespace GameClient
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        private static NetClient _client;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //            using (var game = new Game1())
            //                game.Run();

            //may not need to specify the client's port
            _client = new NetClient(new NetPeerConfiguration("King of Ames"));
            _client.Start();
            //send initial message with login type, this lets the server know its okay to
            //approve the connection
            var outMsg = _client.CreateMessage();
            outMsg.Write((byte)PacketTypes.Login);
            //connect to the server TODO have the server not be hardcoded
            _client.Connect("localhost", 6969, outMsg);

            //Wait for an initial status confirmation from the server
            waitForInitialMessage();

            //Client input loop
            //TODO make a thread to listen for incoming messages from the server

            Thread recieve = new Thread(recieveLoop);
            recieve.Start();
            while (true)
            {
                Console.WriteLine("Commands: 'list', 'new', 'del', 'newGame'");
                var command = Console.ReadLine();
                if (command == "new")
                {
                    newUser();
                }
                else if (command == "del")
                {
                    delUser();
                }
                else if (command == "list")
                {
                    listUsers();
                }
                else if (command == "newGame")
                {
                    newGame();
                }
                else if (command == "joinGame")
                {
                    joinGame();
                }
                else
                {
                    Console.WriteLine("Invalid command, try again.");
                }
            }
        }

        public static void sendLoop()
        {
            while (true)
            {

            }
        }

        public static void recieveLoop()
        {
            while (true)
            {
                NetIncomingMessage inc;
                if ((inc = _client.ReadMessage()) == null) continue;
                switch (inc.MessageType)
                {
                    case NetIncomingMessageType.Error:
                        break;
                    case NetIncomingMessageType.Data:
                        var type = inc.ReadByte();
                        if (type == (byte)PacketTypes.ListUsers)
                        {
                            Console.WriteLine(inc.ReadString());
                        }
                        else if (type == (byte)PacketTypes.chat)
                        {
                            Console.WriteLine(inc.ReadString());
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

        //Sends the server new user information to be added to the database
        public static void newUser()
        {
            Console.Write("What is your name? ");
            var name = Console.ReadLine();
            Console.WriteLine(name);
            var ip = GetLocalIPAddress();
            Console.WriteLine(ip);
            NetOutgoingMessage sendMsg = _client.CreateMessage();
            sendMsg.Write((byte)PacketTypes.NewUser);
            sendMsg.Write(name);
            sendMsg.Write(ip);

            _client.SendMessage(sendMsg, NetDeliveryMethod.ReliableOrdered);
        }

        public static void delUser()
        {
            Console.Write("Type the name you would like to remove: ");
            var name = Console.ReadLine();
            Console.WriteLine(name);
            var ip = GetLocalIPAddress();
            Console.WriteLine(ip);
            NetOutgoingMessage sendMsg = _client.CreateMessage();
            sendMsg.Write((byte)PacketTypes.delUser);
            sendMsg.Write(name);
            sendMsg.Write(ip);

            _client.SendMessage(sendMsg, NetDeliveryMethod.ReliableOrdered);
        }

        //Asks the server to list all of the current database entries
        public static void listUsers()
        {
            NetOutgoingMessage sendMsg = _client.CreateMessage();
            sendMsg.Write((byte)PacketTypes.ListUsers);
            _client.SendMessage(sendMsg, NetDeliveryMethod.ReliableOrdered);
        }

        public static void newGame()
        {
            NetOutgoingMessage createMsg = _client.CreateMessage();
            createMsg.Write((byte)PacketTypes.newGame);
            createMsg.Write(GetLocalIPAddress());
            _client.SendMessage(createMsg, NetDeliveryMethod.ReliableOrdered);

            Console.Clear();
            Console.WriteLine("You have created a new lobby.\nYou can chat with other users who connect, or type 'quit' to close the lobby.\ncls clears the console.");
            Thread gameRead = new Thread(recieveLoop);
            gameRead.Start();
            while (true)
            {
                var cmd = Console.ReadLine();
                if (cmd == "quit")
                {
                    NetOutgoingMessage outMsg = _client.CreateMessage();
                    outMsg.Write((byte)PacketTypes.close);
                    outMsg.Write(GetLocalIPAddress());
                    _client.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered);
                    Console.Clear();
                    break;
                }
                else if (cmd == "cls")
                {
                    Console.Clear();
                    Console.WriteLine("You have created a new lobby.\nYou can chat with other users who connect, or type 'quit' to close the lobby.\ncls clears the console.");
                }
                else if (cmd != "")
                {
                    NetOutgoingMessage outMsg = _client.CreateMessage();
                    outMsg.Write((byte)PacketTypes.chat);
                    outMsg.Write(cmd);
                    _client.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered);
                }
            }
        }

        public static void joinGame()
        {
            NetOutgoingMessage createMsg = _client.CreateMessage();
            createMsg.Write((byte)PacketTypes.joinGame);
            createMsg.Write(GetLocalIPAddress());
            _client.SendMessage(createMsg, NetDeliveryMethod.ReliableOrdered);

            Console.Clear();
            Console.WriteLine("You have joined a lobby.\nYou can chat with other users who connect, or type 'quit' to leave the lobby.\ncls clears the console.");
            Thread gameRead = new Thread(recieveLoop);
            gameRead.Start();
        }

        public static void waitForInitialMessage()
        {
            var ready = false;
            NetIncomingMessage inc;

            while (!ready)
            {
                if ((inc = _client.ReadMessage()) == null) continue;
                switch (inc.MessageType)
                {
                    //Waits for the server to respond with connected TODO be more picky about this
                    case NetIncomingMessageType.StatusChanged:
                        Console.WriteLine("Server status changed: " + inc.SenderConnection.Status);
                        ready = true;
                        break;

                    default:
                        Console.WriteLine("Unknown message with message type: " + inc.MessageType);
                        break;
                }
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
        close,
        Welcome,

    }
}