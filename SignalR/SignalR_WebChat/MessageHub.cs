using System;
using System.Collections.Generic;
using System.Linq;
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
            Clients.All.MessageREceiver(DateTime.Now);
        }
    }
}