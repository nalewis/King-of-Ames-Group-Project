using System;
using MySql.Data.MySqlClient;
using System.Data;
using Controllers.User;
using Controllers.Helpers;

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
                //TODO catch exception for non-unique player name and return false
                throw;
            }

            connection.Close();
            return true;
        }

        public static bool login(string user, string pass, string ip)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            MySqlCommand command;
            connection.Open();
            DataSet ds;
            try
            {
                command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM User_List WHERE Username = @user AND Password = @pass";
                command.Parameters.AddWithValue("@user", user);
                command.Parameters.AddWithValue("@pass", pass);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                ds = new DataSet();
                adapter.Fill(ds);
                if(ds.Tables[0].Rows.Count != 0)
                {
                    User.username = ds.Tables[0].Rows[0]["Username"].ToString();
                    User.localIp = Helpers.GetLocalIPAddress();
                    connection.Close();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;
        }

        public static bool createServer(string hostid, string hostip)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            MySqlCommand command;
            connection.Open();
            try
            {
                command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Server_List (Host, Host_IP) VALUES (@hostid, @hostip, 'Creating')";
                command.Parameters.AddWithValue("@hostid", hostid);
                command.Parameters.AddWithValue("@hostip", hostip);
            }
            catch (Exception)
            {
                throw;
            }

            connection.Close();
            return true;
        }

        public static bool deleteServer(string hostid)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            MySqlCommand command;
            connection.Open();
            try
            {
                command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Server_List WHERE Host = '" + hostid + "'";
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
        public DataSet getServers()
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();
            DataSet ds;

            try
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Server_List";
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                ds = new DataSet();
                adapter.Fill(ds);
            }
            catch (Exception)
            {
                throw;
            }

            connection.Close();
            return ds;
        }
    }
}
