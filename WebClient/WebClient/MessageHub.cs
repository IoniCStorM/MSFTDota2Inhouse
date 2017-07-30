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
        public const bool isDebugOn = true;
        public const uint MAXPLAYERINGAME = 4;

        private struct Player
        {
            public string UserName;
            public string UserID;
            public uint MMR;
            public uint Win;
            public uint Loss;
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

        public void BroadCastAllMessageDebug(string message)
        {
            if (isDebugOn)
            {
                Clients.All.ChatMessageReceiver(message + "\n");
            }
        }

        public void BroadCastMessage(string username, string message)
        {
            Clients.All.ChatMessageReceiver(username + ":" + message + "\n");
        }

        public void CreateMatch()
        {
            BroadCastAllMessageDebug("Lobby is full, creating match!\n");
            List<Player> TeamA = new List<Player>(); 
            List<Player> TeamB = new List<Player>();

            // Get the MMR of all the players in the queue

            // Adding players into teams in the following manner:
            // teamA[0] = Player0, teamA[1] = Player3, teamA[2] = Player4, teamA[3] = Player7, teamA[4] = Player8
            // teamB[0] = Player1, teamB[1] = Player2, teamB[2] = Player5, teamB[3] = Player6, teamA[4] = Player9

            for (int i=0,j=1; i<g_MatchQueue.Count; i++)
            {
                if(j < 2)
                {
                    TeamA.Add(g_MatchQueue[i]);
                }
                else
                {
                    TeamB.Add(g_MatchQueue[i]);
                }

                if(j==3)
                {
                    j = 0;
                }
            }

            // Send SQL to create the game

            // Notify clients a new game has been created, and empty the player queue list

            BroadCastAllMessageDebug("Game 456 has been created");
            Clients.All.UpdateGameListReceiver("456", true);

        }

        public void PopulateCurrentPlayerQueueList()
        {
            for(int i = 0; i < g_MatchQueue.Count; i++)
            {
                Clients.Caller.UpdatePlayerQueueListReceiver(g_MatchQueue[i].UserName, true);
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

        public void SignUpForMatch(string UserName, bool fSignup)
        {
            string queryString = "SELECT `basicinfo`.`UserID`,`MMR`,`Win`,`Loss` FROM `basicinfo` RIGHT JOIN `playerdetails` ON `basicinfo`.`UserID` = `playerdetails`.`UserID` WHERE `basicinfo`.`username` LIKE '" + UserName + "'";
            string UserID = "";
            uint MMR = 0;
            uint Win = 0;
            uint Loss = 0;

            MySqlCommand command = new MySqlCommand(queryString, g_MySQLConnection);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                UserID = reader[0].ToString();
                MMR = Convert.ToUInt32(reader[1].ToString());
                Win = Convert.ToUInt32(reader[2].ToString());
                Loss = Convert.ToUInt32(reader[3].ToString());
            }
            reader.Close();

            Player Player = new Player();
            Player.UserName = UserName;
            Player.UserID = UserID;
            Player.MMR = MMR;
            Player.Win = Win;
            Player.Loss = Loss;

            if (fSignup)
            {
                if (g_MatchQueue.Count < MAXPLAYERINGAME)
                {
                    g_MatchQueue.Add(Player);

                    Clients.All.UpdatePlayerQueueListReceiver(UserName, true);
                    BroadCastAllMessageDebug(String.Format("{0} ({1} {2}-{3}) has signed up!\n", UserName, MMR, Win, Loss));

                    if (g_MatchQueue.Count == MAXPLAYERINGAME)
                    {
                        CreateMatch();
                    }
                }
                else
                {
                    Clients.Caller.ChatMessageReceiver("Lobby is full, please wait.\n");
                }
            }
            else
            {
                g_MatchQueue.Remove(Player);

                Clients.All.UpdatePlayerQueueListReceiver(UserName, false);
                BroadCastAllMessageDebug(UserName + " has abandonned!\n");
            }
            
        }

        public void ConnectUser(string UserName)
        {
            BroadCastAllMessageDebug(UserName + " has connected.\n");
            g_Connections.Add(UserName, Context.ConnectionId);

            // Get player UserID
            string UserID = "";
            UInt32 MMR = 0;
            string queryString = "SELECT UserID FROM `basicinfo` WHERE `username` LIKE '" + UserName + "'";
            MySqlCommand command = new MySqlCommand(queryString, g_MySQLConnection);
            if (g_MySQLConnection.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    g_MySQLConnection.Open();
                } catch (Exception e)
                {
                    BroadCastAllMessageDebug("Connection Failed:" + e.Message + "\n");
                }
            }
            
            if (g_MySQLConnection.State != System.Data.ConnectionState.Open)
            {
                BroadCastAllMessageDebug("Cannot open to database\n");
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
                reader.Close();
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
                reader.Close();
            }

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

            BroadCastAllMessageDebug(userName + " has disconnected.\n");
            Clients.All.UpdateOnlinePlayerListReceiver(userName, false);
            SignUpForMatch(userName, false);

            return base.OnDisconnected(stopCalled);
        }
    }
}