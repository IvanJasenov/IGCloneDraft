using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PresenceTracker
    {
        // collectionIds are strings so we store then as List of strings
        private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();


        public Task UserConnected(string username, string connectionId)
        {
            // dictionary isnt thread save so we lock the dictionary untill we finish doing what we are doing inside here
            // t comes with a scaliability problem, only on euser at a time can access this
            lock(OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    OnlineUsers.Add(username, new List<string> { connectionId });
                }
            }

            return Task.CompletedTask;
        }

        public Task UserDisconnected(string username, string connectionId)
        {
            lock(OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(username))
                {
                    return Task.CompletedTask;
                }

                OnlineUsers[username].Remove(connectionId);
                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                }
            }

            return Task.CompletedTask;
        }


        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock(OnlineUsers)
            {
                onlineUsers = OnlineUsers.OrderBy(el => el.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }

    }
}
