using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace WebClient
{
    public class MessageHub : Hub
    {
        private struct Player
        {
            public string strUsername;
            public int intMMR;
        };
        static List<Player> MatchQueue = new List<Player>(10);
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

        public override Task OnDisconnected(bool stopCalled)
        {
            Clients.All.MessageReceiver(DateTime.Now);
            return base.OnDisconnected(stopCalled);
        }
    }
}