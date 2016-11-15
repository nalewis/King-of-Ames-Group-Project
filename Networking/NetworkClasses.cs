using System;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;

namespace Networking
{
    /// <summary>
    /// This class contains the various functions for access the MySQL database
    /// </summary>
    public class NetworkClasses
    {
        //Information to access the MySQL database
        static string connectString = "Server=10.25.71.66;Database=db309yt01;Uid=dbu309yt01;Pwd=ZuuYea5cBtZ;";
        
        /// <summary>
        /// Using the inputs from the signup form, opens a connection to the database, check if username exists, if not, adds user to list
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="ip"></param>
        public static bool CreateUser(string user, string pass, string ip)
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
                //catch exception for non-unique player name and return false
                return false;
            }
            DataSet ds = new DataSet();
            command.CommandText = "SELECT * FROM User_List WHERE Username = @username";
            command.Parameters.AddWithValue("@username", user);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            adapter.Fill(ds);
            command = connection.CreateCommand();
            command.CommandText = "INSERT INTO User_Stats (Player_ID, Games_Joined, Games_Hosted) VALUES (@id,0,0)";
            command.Parameters.AddWithValue("@id", ds.Tables[0].Rows[0]["Player_ID"]);
            command.ExecuteNonQuery();
            connection.Close();
            return true;
        }

        /// <summary>
        /// Using inputs from the login form, opens a connection to the database, checks if username exsits, if it does, checks if password is correct
        /// Returns false if any checks fail
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool Login(string user, string pass, string ip)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM User_List WHERE Username = @user AND Password = @pass";
            command.Parameters.AddWithValue("@user", user);
            command.Parameters.AddWithValue("@pass", pass);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            var ds = new DataSet();
            adapter.Fill(ds);
            if (ds.Tables[0].Rows.Count != 0)
            {
                //update the players ip
                UpdateIp(ds.Tables[0].Rows[0]["Player_ID"].ToString(), ip);

                User.Username = ds.Tables[0].Rows[0]["Username"].ToString();
                User.LocalIp = ip;
                User.Id = ds.Tables[0].Rows[0]["Player_ID"].ToString();
                User.Character = ds.Tables[0].Rows[0]["_Character"].ToString();
                connection.Close();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates the players IP in the database
        /// This is called when the user succesfully logs in
        /// </summary>
        /// <param name="playerid"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool UpdateIp(string playerid, string ip)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "UPDATE User_List SET Local_IP = @ip WHERE Player_ID = @playerid";
            command.Parameters.AddWithValue("@ip", ip);
            command.Parameters.AddWithValue("@playerid", playerid);
            command.ExecuteNonQuery();

            connection.Close();
            return true;
        }

        /// <summary>
        /// Checks if the player id is the host of a server (for use by the exit function)
        /// </summary>
        /// <param name="hostid"></param>
        /// <returns></returns>
        public static bool IsHosting(string hostid)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Server_List WHERE Host = @hostid";
            command.Parameters.AddWithValue("@hostid", hostid);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            var ds = new DataSet();
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

        /// <summary>
        /// Creates a new server entry in the database when a user hosts a game
        /// </summary>
        /// <param name="hostid"></param>
        /// <param name="hostip"></param>
        /// <returns></returns>
        public static bool CreateServer(string hostid, string hostip)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Server_List (Host, Host_IP, Status) VALUES (@hostid, @hostip, 'Creating')";
            command.Parameters.AddWithValue("@hostid", hostid);
            command.Parameters.AddWithValue("@hostip", hostip);
            command.ExecuteNonQuery();

            connection.Close();
            return true;
        }

        /// <summary>
        /// Deletes server entry from the database when the host closes the server
        /// </summary>
        /// <param name="hostid"></param>
        /// <returns></returns>
        public static bool DeleteServer(string hostid)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Server_List WHERE Host = '" + hostid + "'";
            command.ExecuteNonQuery();

            connection.Close();
            return true;
        }

        /// <summary>
        /// Gets the server information from the database using a host ID & IP
        /// </summary>
        /// <param name="hostid"></param>
        /// <param name="hostip"></param>
        /// <returns>Dataset containing the server ID, host ID, host IP, and connected player IDs</returns>
        public static DataSet GetServer(string hostid, string hostip)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Server_List WHERE Host = @hostid AND Host_IP = @hostip";
            command.Parameters.AddWithValue("@hostid", hostid);
            command.Parameters.AddWithValue("@hostip", hostip);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            var ds = new DataSet();
            adapter.Fill(ds);

            connection.Close();
            return ds;
        }

        /// <summary>
        /// Gets the server information from the database using a host IP
        /// </summary>
        /// <param name="hostip"></param>
        /// <returns>>Dataset containing the server ID, host ID, host IP, and connected player IDs</returns>
        public static DataSet GetServer(string hostip)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Server_List WHERE Host_IP = @hostip";
            command.Parameters.AddWithValue("@hostip", hostip);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            var ds = new DataSet();
            adapter.Fill(ds);

            connection.Close();
            return ds;
        }

        /// <summary>
        /// Gets the server info of all servers in the database
        /// </summary>
        /// <returns>Dataset containing server info of all servers</returns>
        public static DataSet GetServers()
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Server_List WHERE Status ='Creating'";
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            var ds = new DataSet();
            adapter.Fill(ds);

            connection.Close();
            return ds;
        }

        /// <summary>
        /// Gets player info using a given player ID
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns>Dataset containing player info</returns>
        public static DataSet GetPlayer(int playerId)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM User_List WHERE Player_ID = @Player_ID";
            command.Parameters.AddWithValue("@Player_ID", playerId);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            var ds = new DataSet();
            adapter.Fill(ds);

            connection.Close();
            return ds;
        }

        /// <summary>
        /// Updates the players character field to the input
        /// </summary>
        /// <param name="playerid"></param>
        /// <param name="character"></param>
        public static void UpdateCharacter(string playerid, string character)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "UPDATE User_List SET _Character = @character WHERE Player_ID = @playerid";
            command.Parameters.AddWithValue("@character", character);
            command.Parameters.AddWithValue("@playerid", playerid);
            command.ExecuteNonQuery();

            connection.Close();
        }

        /// <summary>
        /// Adds the new player into the server info
        /// </summary>
        /// <param name="hostip"></param>
        /// <param name="playerid"></param>
        /// <returns></returns>
        public static bool JoinServer(string hostip, string playerid)
        {
            var openSpot = GetNextAvailableSpot(hostip);
            if (openSpot != -1)
            {
                MySqlConnection connection = new MySqlConnection(connectString);
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "UPDATE Server_List SET Player_" + openSpot + " = @playerid WHERE Host_IP = @hostip";
                command.Parameters.AddWithValue("@hostip", hostip);
                command.Parameters.AddWithValue("@playerid", playerid);
                command.ExecuteNonQuery();

                connection.Close();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the lowest numbered available spot from the server information
        /// </summary>
        /// <param name="hostip"></param>
        /// <returns>lowest spot in server 2-6</returns>
        public static int GetNextAvailableSpot(string hostip)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Server_List WHERE Host_IP = @hostip";
            command.Parameters.AddWithValue("@hostip", hostip);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            var ds = new DataSet();
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

            connection.Close();
            return -1;
        }

        /// <summary>
        /// Finds the player in the server list to be removed
        /// </summary>
        /// <param name="hostip"></param>
        /// <param name="playerid"></param>
        public static void FindRemovePlayer(string hostip, string playerid)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();
            int remove = -1;

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Server_List WHERE Host_IP = @hostip";
            command.Parameters.AddWithValue("@hostip", hostip);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            var ds = new DataSet();
            adapter.Fill(ds);
            if (ds.Tables[0].Rows.Count != 0)
            {
                //var stuff = String.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_2"].ToString());
                //var thing = ds.Tables[0].Rows[0]["Player_2"].ToString();
                if (string.CompareOrdinal(ds.Tables[0].Rows[0]["Player_2"].ToString(), playerid) == 0)
                {
                    connection.Close();
                    remove = 2;
                }
                else if (string.CompareOrdinal(ds.Tables[0].Rows[0]["Player_3"].ToString(), playerid) == 0)
                {
                    connection.Close();
                    remove = 3;
                }
                else if (string.CompareOrdinal(ds.Tables[0].Rows[0]["Player_4"].ToString(), playerid) == 0)
                {
                    connection.Close();
                    remove = 4;
                }
                else if (string.CompareOrdinal(ds.Tables[0].Rows[0]["Player_5"].ToString(), playerid) == 0)
                {
                    connection.Close();
                    remove = 5;
                }
                else if (String.CompareOrdinal(ds.Tables[0].Rows[0]["Player_6"].ToString(), playerid) == 0)
                {
                    connection.Close();
                    remove = 6;
                }
                else
                {
                    connection.Close();
                }
            }

            connection.Close();
            RemovePlayer(hostip, remove);
        }

        /// <summary>
        /// Removes player with matching ID from the server information
        /// </summary>
        /// <param name="hostip"></param>
        /// <param name="playerPosition"></param>
        public static void RemovePlayer(string hostip, int playerPosition)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Server_List SET Player_" + playerPosition + " = null WHERE Host_IP = @hostip";
            command.Parameters.AddWithValue("@hostip", hostip);
            command.ExecuteNonQuery();

            connection.Close();
        }

        /// <summary>
        /// Checks that every player in the server has selected a character
        /// </summary>
        /// <param name="players"></param>
        /// <returns>false if any player hasn't selected a character, true otherwise</returns>
        public static bool CheckReady(List<int> players)
        {
            List<DataSet> playersList = new List<DataSet>();
            foreach (int player in players)
            {
                playersList.Add(GetPlayer(player));
            }
            foreach (DataSet player in playersList)
            {
                if(String.IsNullOrEmpty(player.Tables[0].Rows[0]["_Character"].ToString())) { return false; }
            }
            return true;
        }

        /// <summary>
        /// Gets the current number of players in the server
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns>number of players in the server</returns>
        public static int GetNumPlayers(int serverId)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();
            int count = -1;

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Server_List WHERE Server_ID = @Server_ID";
            command.Parameters.AddWithValue("@Server_ID", serverId);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            var ds = new DataSet();
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

            connection.Close();
            return count;
        }

        /// <summary>
        /// Gets an int array of the player IDs currently connected to the Host
        /// </summary>
        /// <param name="hostip"></param>
        /// <returns></returns>
        public static int[] GetPlayerIDs(string hostip)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();
            int[] players = { -1, -1, -1, -1, -1, -1 };

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Server_List WHERE Host_IP = @hostip";
            command.Parameters.AddWithValue("@hostip", hostip);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            var ds = new DataSet();
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

            connection.Close();
            return players;
        }

        public static void UpdatePlayerStat(string playerid, string stat, int value)
        {
            MySqlConnection connection = new MySqlConnection(connectString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "UPDATE User_Stats SET " + stat + " = " + stat + " + @value WHERE Player_ID = @playerid";
            command.Parameters.AddWithValue("@value", value);
            command.Parameters.AddWithValue("@playerid", playerid);
            command.ExecuteNonQuery();

            connection.Close();
        }
    }
}
