using System;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;
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
                if (ds.Tables[0].Rows.Count != 0)
                {
                    //update the players ip
                    updateIP(ds.Tables[0].Rows[0]["Player_ID"].ToString(), ip);

                    User.username = ds.Tables[0].Rows[0]["Username"].ToString();
                    User.localIp = ip;
                    User.id = ds.Tables[0].Rows[0]["Player_ID"].ToString();
                    User.character = ds.Tables[0].Rows[0]["_Character"].ToString();
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

        public static bool updateIP(string playerid, string ip)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            MySqlCommand command;
            connection.Open();
            try
            {
                command = connection.CreateCommand();
                command.CommandText = "UPDATE User_List SET Local_IP = @ip WHERE Player_ID = @playerid";
                command.Parameters.AddWithValue("@ip", ip);
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

        public static DataSet getServer(string hostip)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();
            DataSet ds;

            try
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Server_List WHERE Host_IP = @hostip";
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

        public static bool updateCharacter(string playerid, string character)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            MySqlCommand command;
            connection.Open();
            try
            {
                command = connection.CreateCommand();
                command.CommandText = "UPDATE User_List SET _Character = @character WHERE Player_ID = @playerid";
                command.Parameters.AddWithValue("@character", character);
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

        public static bool joinServer(string hostip, string playerid)
        {
            var openSpot = getNextAvailableSpot(hostip);
            if (openSpot != -1)
            {
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
            else
            {
                return false;
            }
        }

        public static int getNextAvailableSpot(string hostip)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();
            DataSet ds;

            try
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Server_List WHERE Host_IP = @hostip";
                command.Parameters.AddWithValue("@hostip", hostip);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                ds = new DataSet();
                adapter.Fill(ds);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    //var stuff = String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_2"].ToString());
                    //var thing = ds.Tables[0].Rows[0]["Player_2"].ToString();
                    if (String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_2"].ToString()))
                    {
                        connection.Close();
                        return 2;
                    }
                    else if (String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_3"].ToString()))
                    {
                        connection.Close();
                        return 3;
                    }
                    else if (String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_4"].ToString()))
                    {
                        connection.Close();
                        return 4;
                    }
                    else if (String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_5"].ToString()))
                    {
                        connection.Close();
                        return 5;
                    }
                    else if (String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_6"].ToString()))
                    {
                        connection.Close();
                        return 6;
                    }
                    else
                    {
                        connection.Close();
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

        public static void findRemovePlayer(string hostip, string playerid)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();
            DataSet ds;
            int remove = -1;

            try
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Server_List WHERE Host_IP = @hostip";
                command.Parameters.AddWithValue("@hostip", hostip);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                ds = new DataSet();
                adapter.Fill(ds);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    //var stuff = String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_2"].ToString());
                    //var thing = ds.Tables[0].Rows[0]["Player_2"].ToString();
                    if (String.Compare(ds.Tables[0].Rows[0]["Player_2"].ToString(), playerid) == 0)
                    {
                        connection.Close();
                        remove = 2;
                    }
                    else if (String.Compare(ds.Tables[0].Rows[0]["Player_3"].ToString(), playerid) == 0)
                    {
                        connection.Close();
                        remove = 3;
                    }
                    else if (String.Compare(ds.Tables[0].Rows[0]["Player_4"].ToString(), playerid) == 0)
                    {
                        connection.Close();
                        remove = 4;
                    }
                    else if (String.Compare(ds.Tables[0].Rows[0]["Player_5"].ToString(), playerid) == 0)
                    {
                        connection.Close();
                        remove = 5;
                    }
                    else if (String.Compare(ds.Tables[0].Rows[0]["Player_6"].ToString(), playerid) == 0)
                    {
                        connection.Close();
                        remove = 6;
                    }
                    else
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            connection.Close();
            removePlayer(hostip, remove);
        }

        public static void removePlayer(string hostip, int playerPosition)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();

            try
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE Server_List SET Player_" + playerPosition + " = null WHERE Host_IP = @hostip";//TODO maybe set to null?
                command.Parameters.AddWithValue("@hostip", hostip);
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }

            connection.Close();
        }

        public static bool checkReady(List<int> players)
        {
            List<DataSet> players_list = new List<DataSet>();
            foreach (int player in players)
            {
                players_list.Add(getPlayer(player));
            }
            foreach (DataSet player in players_list)
            {
                if(String.IsNullOrEmpty(player.Tables[0].Rows[0]["_Character"].ToString())) { return false; }
            }
            return true;
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

        public static int[] getPlayerIDs(string hostip)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();
            DataSet ds;
            int[] players = new int[6] { -1, -1, -1, -1, -1, -1 };

            try
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Server_List WHERE Host_IP = @hostip";
                command.Parameters.AddWithValue("@hostip", hostip);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                ds = new DataSet();
                adapter.Fill(ds);
                connection.Close();

                if (ds.Tables[0].Rows.Count != 0)
                {
                    players[0] = Int32.Parse(ds.Tables[0].Rows[0]["Host"].ToString());

                    if (!String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_2"].ToString()))
                    {
                        players[1] = Int32.Parse(ds.Tables[0].Rows[0]["Player_2"].ToString());
                    }
                    if (!String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_3"].ToString()))
                    {
                        players[2] = Int32.Parse(ds.Tables[0].Rows[0]["Player_3"].ToString());
                    }
                    if (!String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_4"].ToString()))
                    {
                        players[3] = Int32.Parse(ds.Tables[0].Rows[0]["Player_4"].ToString());
                    }
                    if (!String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_5"].ToString()))
                    {
                        players[4] = Int32.Parse(ds.Tables[0].Rows[0]["Player_5"].ToString());
                    }
                    if (!String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_6"].ToString()))
                    {
                        players[5] = Int32.Parse(ds.Tables[0].Rows[0]["Player_6"].ToString());
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            connection.Close();
            return players;
        }
    }
}
