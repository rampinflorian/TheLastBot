using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using ManziBot.Server.Data;

namespace ManziBot.Server.Services
{
    public class TwitchDetectorService
    {
        public Task<List<SocketGuildUser>> GetStreamingUsersAsync(List<SocketGuildUser> users)
        {
            var streamingUsers = new List<SocketGuildUser>();
            streamingUsers.AddRange(users.Where(m => !m.IsBot && m.Activity is not null));

            return Task.FromResult(streamingUsers);
        }
    }
}