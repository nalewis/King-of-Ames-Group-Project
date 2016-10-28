using System.Collections.Generic;
using GamePieces.Dice;
using GamePieces.Session;
using System.Web.Script.Serialization;
using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Specialized;
using System.Text;

namespace Controllers.test
{
    public static class Test
    {
        public static void StartGame(List<string> names)
        {
            Game.StartGame(names);
        }

        public static List<Die> GetDice()
        {
            return DiceRoller.Rolling;
        }
    }

    public static class Helpers
    {
        public static string ToJSON(this object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        //specifically made for the listServers function at the moment
        public static ServerDetails[] FromJSON(string json)
        {
            JavaScriptSerializer deserializer = new JavaScriptSerializer();
            ServerDetails[] list = deserializer.Deserialize<ServerDetails[]>(json);
            //Console.WriteLine(list[0].playerDetails);
            return list;
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

        public static string WebMessage(NameValueCollection data)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var result = wc.UploadValues("http://proj-309-yt-01.cs.iastate.edu/login.php", "POST", data);
                return Encoding.ASCII.GetString(result);
                //Console.WriteLine("\nResponse received was :\n{0}", encresult);
            }
        }
    }

    public class PlayerDetails
    {

        public string name = "";
        public string ip = "";
        public string character = "";
    }

    public class ServerDetails
    {
        /*public ServerDetails(string json)
        {
            JObject jObject = JObject.Parse(json);
            JToken jUser = jObject["user"];
            name = (string)jUser["name"];
        }*/

        public string hostname { get; set; }
        public string hostip { get; set; }
        //TODO
        //public Array playerDetails { get; set; }
    }

}

namespace Controllers.User
{
    public static class User
    {
        public static string username = "";
        public static string localIp = "";
    }
}