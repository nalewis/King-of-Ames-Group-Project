using System;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace Networking
{
    /// <summary>
    /// This class contains the various functions for access the MySQL database
    /// </summary>
    public class NetworkClasses
    {
        //Information to access the MySQL database
        private const string ConnectString = "Server=10.25.71.66;Database=db309yt01;Uid=dbu309yt01;Pwd=ZuuYea5cBtZ;";

        /// <summary>
        /// Using the inputs from the signup form, opens a connection to the database, check if username exists, if not, adds user to list
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="ip"></param>
        public static bool CreateUser(string user, string pass, string ip)
        {
            var connection = new MySqlConnection(ConnectString);
            MySqlCommand command;
            connection.Open();
            pass = StringCipher.Encrypt(pass, "thomas");
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
            var ds = new DataSet();
            command.CommandText = "SELECT * FROM User_List WHERE Username = @username";
            command.Parameters.AddWithValue("@username", user);
            var adapter = new MySqlDataAdapter(command);
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
            var connection = new MySqlConnection(ConnectString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM User_List WHERE Username = @user";
            command.Parameters.AddWithValue("@user", user);
            var adapter = new MySqlDataAdapter(command);
            var ds = new DataSet();
            adapter.Fill(ds);
            if (ds.Tables[0].Rows.Count == 0) return false;
            var dbpass = StringCipher.Decrypt(ds.Tables[0].Rows[0]["password"].ToString(), "thomas");
            if (dbpass != pass) return false;
            //update the players ip
            UpdateIp(ds.Tables[0].Rows[0]["Player_ID"].ToString(), ip);

            User.Username = ds.Tables[0].Rows[0]["Username"].ToString();
            User.LocalIp = ip;
            User.Id = ds.Tables[0].Rows[0]["Player_ID"].ToString();
            User.Character = ds.Tables[0].Rows[0]["_Character"].ToString();
            connection.Close();
            return true;
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
            var connection = new MySqlConnection(ConnectString);
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
            var connection = new MySqlConnection(ConnectString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Server_List WHERE Host = @hostid";
            command.Parameters.AddWithValue("@hostid", hostid);
            var adapter = new MySqlDataAdapter(command);
            var ds = new DataSet();
            adapter.Fill(ds);
            connection.Close();
            return ds.Tables[0].Rows.Count != 0;
        }

        /// <summary>
        /// Creates a new server entry in the database when a user hosts a game
        /// </summary>
        /// <param name="hostid"></param>
        /// <param name="hostip"></param>
        /// <returns></returns>
        public static bool CreateServer(string hostid, string hostip)
        {
            var connection = new MySqlConnection(ConnectString);
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
            var connection = new MySqlConnection(ConnectString);
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
            var connection = new MySqlConnection(ConnectString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Server_List WHERE Host = @hostid AND Host_IP = @hostip";
            command.Parameters.AddWithValue("@hostid", hostid);
            command.Parameters.AddWithValue("@hostip", hostip);
            var adapter = new MySqlDataAdapter(command);
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
            var connection = new MySqlConnection(ConnectString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Server_List WHERE Host_IP = @hostip";
            command.Parameters.AddWithValue("@hostip", hostip);
            var adapter = new MySqlDataAdapter(command);
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
            var connection = new MySqlConnection(ConnectString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Server_List WHERE Status ='Creating'";
            var adapter = new MySqlDataAdapter(command);
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
            var connection = new MySqlConnection(ConnectString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM User_List WHERE Player_ID = @Player_ID";
            command.Parameters.AddWithValue("@Player_ID", playerId);
            var adapter = new MySqlDataAdapter(command);
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
            var connection = new MySqlConnection(ConnectString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "UPDATE User_List SET _Character = @character WHERE Player_ID = @playerid";
            command.Parameters.AddWithValue("@character", character);
            command.Parameters.AddWithValue("@playerid", playerid);
            command.ExecuteNonQuery();

            connection.Close();
        }

        /// <summary>
        /// Updates the server status based on player count and game status
        /// </summary>
        /// <param name="status"></param>
        /// <param name="id"></param>
        public static void UpdateServerStatus(string status, string id)
        {
            var connection = new MySqlConnection(ConnectString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "UPDATE Server_List SET Status = @status WHERE Host = @id";
            command.Parameters.AddWithValue("@status", status);
            command.Parameters.AddWithValue("@id", id);
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
                var connection = new MySqlConnection(ConnectString);
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
            var connection = new MySqlConnection(ConnectString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Server_List WHERE Host_IP = @hostip";
            command.Parameters.AddWithValue("@hostip", hostip);
            var adapter = new MySqlDataAdapter(command);
            var ds = new DataSet();
            adapter.Fill(ds);
            if (ds.Tables[0].Rows.Count != 0)
            {
                for (var i = 2; i < 6; i++)
                {
                    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_" + i].ToString())) continue;
                    connection.Close();
                    return i;
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
            var connection = new MySqlConnection(ConnectString);
            connection.Open();
            var remove = -1;

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Server_List WHERE Host_IP = @hostip";
            command.Parameters.AddWithValue("@hostip", hostip);
            var adapter = new MySqlDataAdapter(command);
            var ds = new DataSet();
            adapter.Fill(ds);
            if (ds.Tables[0].Rows.Count != 0)
            {
                for (var i = 2; i < 6; i++)
                {
                    if (string.CompareOrdinal(ds.Tables[0].Rows[0]["Player_" + i].ToString(), playerid) == 0) continue;
                    remove = i;
                    break;
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
            var connection = new MySqlConnection(ConnectString);
            connection.Open();

            var command = connection.CreateCommand();
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
            var playersList = players.Select(GetPlayer).ToList();
            return playersList.All(player => !string.IsNullOrEmpty(player.Tables[0].Rows[0]["_Character"].ToString()));
        }

        /// <summary>
        /// Gets the current number of players in the server
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns>number of players in the server</returns>
        public static int GetNumPlayers(int serverId)
        {
            var connection = new MySqlConnection(ConnectString);
            connection.Open();
            var count = -1;

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Server_List WHERE Server_ID = @Server_ID";
            command.Parameters.AddWithValue("@Server_ID", serverId);
            var adapter = new MySqlDataAdapter(command);
            var ds = new DataSet();
            adapter.Fill(ds);
            connection.Close();

            if (ds.Tables[0].Rows.Count == 0) return count;
            count = 1;
            for (var i = 2; i < 6; i++)
            {
                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_" + i].ToString())) continue;
                count++;
            }

            return count;
        }

        /// <summary>
        /// Gets an int array of the player IDs currently connected to the Host
        /// </summary>
        /// <param name="hostip"></param>
        /// <returns>int array of IDs</returns>
        public static int[] GetPlayerIDs(string hostip)
        {
            var connection = new MySqlConnection(ConnectString);
            connection.Open();
            int[] players = { -1, -1, -1, -1, -1, -1 };

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Server_List WHERE Host_IP = @hostip";
            command.Parameters.AddWithValue("@hostip", hostip);
            var adapter = new MySqlDataAdapter(command);
            var ds = new DataSet();
            adapter.Fill(ds);
            connection.Close();

            if (ds.Tables[0].Rows.Count == 0) return players;
            players[0] = int.Parse(ds.Tables[0].Rows[0]["Host"].ToString());
            for (var i = 2; i < 6; i++)
            {
                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_" + i].ToString()))
                {
                    players[i-1] = int.Parse(ds.Tables[0].Rows[0]["Player_" + i].ToString());
                }
            }
            return players;
        }

        public static void UpdatePlayerStat(string playerid, string stat, int value)
        {
            var connection = new MySqlConnection(ConnectString);
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
