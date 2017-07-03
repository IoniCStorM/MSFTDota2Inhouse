using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SignalR_WebChat
{
    public class MessageHub : Hub
    {
        public void Login(string username)
        {
            // Notify all clients that a user is logged in
            Clients.All.userLoggedIn(username);
        }

        public void SendMessage(string sender, string message)
        {
            // Display the new message by calling displayMessage on all connected clients
            Clients.All.displayMessage(sender, message);
        }

        public void BroadCastServerTime()
        {
            Clients.All.MessageReceiver(DateTime.Now);
        }

        public void BroadCastMessage(string username, string message)
        {
            Clients.All.ChatMessageReceiver(username + ":" + message + "\n");
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Clients.All.MessageReceiver(DateTime.Now);
            BroadCastMessage("A user", "Disconnected \n");
            return base.OnDisconnected(stopCalled);
        }
    }
}