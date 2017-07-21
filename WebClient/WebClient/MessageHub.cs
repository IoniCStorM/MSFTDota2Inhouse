using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

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
        static List<Player> MatchQueue = new List<Player>(10);
        private readonly static ConnectionMapping _connections = new ConnectionMapping();
        public void BroadCastServerTime()
        {
            Clients.All.MessageReceiver(DateTime.Now);
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
            for(int i = 0; i < MatchQueue.Count; i++)
            {
                Clients.Caller.UpdatePlayerQueueListReceiver(MatchQueue[i].strUsername, true);
            }
        }

        public void PopulateCurrentPlayerList()
        {
            Clients.Caller.ChatMessageReceiver("UpdatingCurrentPlayerList\n");
            foreach(string userName in _connections.GetKeys())
            {
                Clients.Caller.ChatMessageReceiver( userName + " is in game.\n");
                Clients.Caller.UpdateOnlinePlayerListReceiver(userName, true);
            }
        }

        public void SignUpForMatch(string username, int MMR)
        {
            Player newPlayer = new Player();
            newPlayer.strUsername = username;
            newPlayer.intMMR = 1500;
            MatchQueue.Add(newPlayer);

            Clients.All.UpdatePlayerQueueListReceiver(username, true);
            Clients.All.ChatMessageReceiver(username + " has signed up!\n");

            if(MatchQueue.Count == 2)
            {
                CreateMatch();
            }
        }

        public void AbandonSignUpForMatch(string username, int MMR)
        {
            Player existingPlayer = new Player();
            existingPlayer.strUsername = username;
            existingPlayer.intMMR = 1500;
            MatchQueue.Remove(existingPlayer);

            Clients.All.UpdatePlayerQueueListReceiver(username, false);
            Clients.All.ChatMessageReceiver(username + " has abandonned!\n");
        }

        public void ConnectUser(string UserName)
        {
            _connections.Add(UserName, Context.ConnectionId);
            Clients.All.ChatMessageReceiver(UserName + " has connected.\n");
            Clients.Others.UpdateOnlinePlayerListReceiver(UserName, true);
        }

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Clients.All.MessageReceiver(DateTime.Now);

            string userName;
            userName = _connections.RemoveConnectionId(Context.ConnectionId);

            Clients.All.ChatMessageReceiver(userName + " has disconnected.\n");
            Clients.All.UpdateOnlinePlayerListReceiver(userName, false);

            return base.OnDisconnected(stopCalled);
        }
    }
}