using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Specialized;

namespace Networking
{
    public static class Helpers
    {
        //Took from stack overflow
        public static string GetLocalIpAddress()
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
            using (var wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var result = wc.UploadValues("http://proj-309-yt-01.cs.iastate.edu/login.php", "POST", data);
                return Encoding.ASCII.GetString(result);
                //Console.WriteLine("\nResponse received was :\n{0}", encresult);
            }
        }
    }
}

namespace Networking
{
    public static class User
    {
        public static string Username = "";
        public static string LocalIp = "";
        public static string Id = "";
        public static string Character = "";
    }
}