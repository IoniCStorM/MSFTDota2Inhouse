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
        public void BroadCastServerTime()
        {
            Clients.All.MessageReceiver(DateTime.Now);
        }

        public void BroadCastMessage(string username, string message)
        {
            Clients.All.ChatMessageReceiver(username + ":" + message + "\n");
        }

        public void SignUpForMatch(string username)
        {
            Clients.All.UpdatePlayerQueueListReceiver(username, true);
            Clients.All.ChatMessageReceiver(username + " has signed up!\n");
        }

        public void AbandonSignUpForMatch(string username)
        {
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