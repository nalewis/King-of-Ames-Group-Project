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
                    User.id = ds.Tables[0].Rows[0]["Player_ID"].ToString();
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

        //Check if the player id is the host of a server (for use by the exit function)
        public static bool isHosting(string hostid)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();
            DataSet ds;

            try
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Server_List WHERE Host = @hostid";
                command.Parameters.AddWithValue("@hostid", hostid);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                ds = new DataSet();
                adapter.Fill(ds);
                connection.Close();
                if (ds.Tables[0].Rows.Count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool createServer(string hostid, string hostip)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            MySqlCommand command;
            connection.Open();
            try
            {
                command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Server_List (Host, Host_IP, Status) VALUES (@hostid, @hostip, 'Creating')";
                command.Parameters.AddWithValue("@hostid", hostid);
                command.Parameters.AddWithValue("@hostip", hostip);
                command.ExecuteNonQuery();
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

        public static DataSet getServer(string hostid, string hostip)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();
            DataSet ds;

            try
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Server_List WHERE Host = @hostid AND Host_IP = @hostip";
                command.Parameters.AddWithValue("@hostid", hostid);
                command.Parameters.AddWithValue("@hostip", hostip);
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

        public static DataSet getServers()
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

        public static DataSet getPlayer(int Player_ID)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();
            DataSet ds;

            try
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM User_List WHERE Player_ID = @Player_ID";
                command.Parameters.AddWithValue("@Player_ID", Player_ID);
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

        public static bool joinServer(string hostip, string playerid)
        {
            var openSpot = getNextAvailableSpot(hostip);
            MySqlConnection connection = new MySqlConnection(connectString);
            MySqlCommand command;
            connection.Open();
            try
            {
                command = connection.CreateCommand();
                command.CommandText = "UPDATE Server_List SET Player_" + openSpot + " = @playerid WHERE Host_IP = @hostip";
                command.Parameters.AddWithValue("@hostip", hostip);
                command.Parameters.AddWithValue("@playerid", playerid);
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }

            connection.Close();
            return true;
        }

        public static int getNextAvailableSpot(string hostip)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();
            DataSet ds;

            try
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Server_List WHERE Host_IP = '" + hostip + "'";
                //command.Parameters.AddWithValue("@Host_IP", hostip);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                ds = new DataSet();
                adapter.Fill(ds);
                connection.Close();
                if (ds.Tables[0].Rows.Count != 0)
                {
                    if(String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_2"].ToString()))
                    {
                        return 2;
                    }
                    else if (String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_3"].ToString()))
                    {
                        return 3;
                    }
                    else if (String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_4"].ToString()))
                    {
                        return 4;
                    }
                    else if (String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_5"].ToString()))
                    {
                        return 5;
                    }
                    else if (String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_6"].ToString()))
                    {
                        return 6;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            connection.Close();
            return -1;
        }

        public static int getNumPlayers(int Server_ID)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();
            DataSet ds;
            int count = -1;

            try
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Server_List WHERE Server_ID = @Server_ID";
                command.Parameters.AddWithValue("@Server_ID", Server_ID);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                ds = new DataSet();
                adapter.Fill(ds);
                connection.Close();

                if (ds.Tables[0].Rows.Count != 0)
                {
                    count = 1;
                    if (!String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_2"].ToString()))
                    {
                        count++;
                    }
                    if (!String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_3"].ToString()))
                    {
                        count++;
                    }
                    if (!String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_4"].ToString()))
                    {
                        count++;
                    }
                    if (!String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_5"].ToString()))
                    {
                        count++;
                    }
                    if (!String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_6"].ToString()))
                    {
                        count++;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            connection.Close();
            return count;
        }
    }
}
