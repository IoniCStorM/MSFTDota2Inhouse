using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using MySql.Data.MySqlClient;


namespace WebClient
{
    public class ConnectionMapping
    {
        //
        // This Dictionary contains 2 strings:
        // The Key string username
        // The Value string connectionId
        //
        private readonly Dictionary<string, string> _connections =
            new Dictionary<string, string>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(string userName, string connectionId)
        {
            lock (_connections)
            {
                if (!_connections.ContainsKey(userName))
                {
                    _connections.Add(userName, connectionId);
                }
            }
        }

        public void Remove(string userName)
        {
            lock (_connections)
            {
                if (_connections.ContainsKey(userName))
                {
                    _connections.Remove(userName);
                }
            }
        }

        public string RemoveConnectionId(string connectionId)
        {
            string userName = "";
            lock (_connections)
            {
                foreach(KeyValuePair<string, string> entry in _connections)
                {
                    if (entry.Value.Equals(connectionId))
                    {
                        userName = entry.Key;
                        _connections.Remove(entry.Key);
                        break;
                    }
                }
            }

            return userName;
        }

        public List<string> GetKeys()
        {
            List<string> Keys = new List<string>(_connections.Keys);
            return Keys;
        }

        public List<string> GetValues()
        {
            List<string> Values = new List<string>(_connections.Values);
            return Values;
        }
    }

    public class MessageHub : Hub
    {
        private struct Player
        {
            public string strUsername;
            public int intMMR;
        };

        static List<Player> g_MatchQueue = new List<Player>(10);
        private readonly static ConnectionMapping g_Connections = new ConnectionMapping();
        static MySqlConnection g_MySQLConnection = new MySqlConnection("server=localhost;Database=userinfo;Uid=WebClient;Pwd=MSFTDota2InHouse;");

        public void BroadCastServerTime()
        {
            Clients.All.MessageReceiver(DateTime.Now);
        }

        public void BroadCastAllMessage(string message)
        {
            Clients.All.ChatMessageReceiver(message + "\n");
        }

        public void BroadCastMessage(string username, string message)
        {
            Clients.All.ChatMessageReceiver(username + ":" + message + "\n");
        }

        public void CreateMatch()
        {
            Clients.All.ChatMessageReceiver("Lobby is full, creating match!\n");
        }

        public void PopulateCurrentPlayerQueueList()
        {
            for(int i = 0; i < g_MatchQueue.Count; i++)
            {
                Clients.Caller.UpdatePlayerQueueListReceiver(g_MatchQueue[i].strUsername, true);
            }
        }

        public void PopulateCurrentPlayerList()
        {
            Clients.Caller.ChatMessageReceiver("UpdatingCurrentPlayerList\n");
            foreach(string userName in g_Connections.GetKeys())
            {
                Clients.Caller.ChatMessageReceiver( userName + " is in game.\n");
                Clients.Caller.UpdateOnlinePlayerListReceiver(userName, true);
            }
        }

        public void SignUpForMatch(string username, bool fSignup)
        {
            Player Player = new Player();
            Player.strUsername = username;
            Player.intMMR = 1500;
            if (fSignup)
            {
                g_MatchQueue.Add(Player);

                Clients.All.UpdatePlayerQueueListReceiver(username, true);
                Clients.All.ChatMessageReceiver(username + " has signed up!\n");

                if (g_MatchQueue.Count == 4)
                {
                    CreateMatch();
                }
            }
            else
            {
                g_MatchQueue.Remove(Player);

                Clients.All.UpdatePlayerQueueListReceiver(username, false);
                Clients.All.ChatMessageReceiver(username + " has abandonned!\n");
            }
            
        }

        public void ConnectUser(string UserName)
        {
            Clients.All.ChatMessageReceiver(UserName + " has connected.\n");
            g_Connections.Add(UserName, Context.ConnectionId);

            // Get player UserID
            string UserID = "";
            UInt32 MMR = 0;
            string queryString = "SELECT * FROM `basicinfo` WHERE `username` LIKE '" + UserName + "'";
            MySqlCommand command = new MySqlCommand(queryString, g_MySQLConnection);
            if (g_MySQLConnection.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    g_MySQLConnection.Open();
                } catch (Exception e)
                {
                    Clients.All.ChatMessageReceiver("Connection Failed:" + e.Message + "\n");
                }
            }
            
            if (g_MySQLConnection.State != System.Data.ConnectionState.Open)
            {
                Clients.All.ChatMessageReceiver("Cannot open to database\n");
            }
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                UserID = reader[0].ToString();
            }
            reader.Close();

            // Get player MMR
            command.CommandText = "SELECT * FROM `playerdetails` WHERE `UserID` LIKE '" + UserID + "'";
            command.CommandType = System.Data.CommandType.Text;
            
            reader = command.ExecuteReader();

            if (!reader.HasRows)
            {
                // No row exists, adding a row
                command.CommandText = "INSERT INTO `playerdetails` (`UserID`, `MMR`, `Win`, `Loss`) VALUES ('" + UserID + "', '1500', '0', '0')";
                command.ExecuteNonQuery();
                MMR = 1500;
            }
            else
            {
                while (reader.Read())
                {
                    MMR = Convert.ToUInt32(reader[1]);
                }
            }
            reader.Close();

            BroadCastAllMessage(UserName + "'s MMR is: " + MMR);

            Clients.Others.UpdateOnlinePlayerListReceiver(UserName, true);
            Clients.Caller.UpdatePlayerMMRReceiver(MMR);
        }

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Clients.All.MessageReceiver(DateTime.Now);

            string userName;
            userName = g_Connections.RemoveConnectionId(Context.ConnectionId);

            Clients.All.ChatMessageReceiver(userName + " has disconnected.\n");
            Clients.All.UpdateOnlinePlayerListReceiver(userName, false);

            return base.OnDisconnected(stopCalled);
        }
    }
}