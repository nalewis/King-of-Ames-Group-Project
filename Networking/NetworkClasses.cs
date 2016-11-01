using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Networking
{
    public class NetworkClasses
    {
        static string connectString = "Server=10.25.71.66;Database=db309yt01;Uid=dbu309yt01;Pwd=ZuuYea5cBtZ;";

        public static bool createUser(string user, string pass, string ip)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            MySqlCommand command;
            connection.Open();
            try
            {
                command = connection.CreateCommand();
                command.CommandText = "INSERT INTO User_List (Username, Password, Local_IP) VALUES (@user,@pass,@ip)";
                command.Parameters.AddWithValue("@user", user);
                command.Parameters.AddWithValue("@pass", pass);
                command.Parameters.AddWithValue("@ip", ip);
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }

            connection.Close();
            return true;
        }

        //Returns the details of all the players in the server hosted by hostname w/ hostip
        public void getPlayers(string hostname, string hostip)
        {

        }
    }
}
