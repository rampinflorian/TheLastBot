using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace ManziBot.Server.Services
{
    public class StreamAnalyzeService
    {
        private readonly DiscordSocketClient _client;
        private readonly TwitchDetectorService _twitchDetectorService;
        private readonly IConfiguration _configuration;

        private  ulong GuildId = 0;
        private  ulong ChannelId = 0;

        public StreamAnalyzeService(DiscordSocketClient client, TwitchDetectorService twitchDetectorService, IConfiguration configuration)
        {
            _client = client;
            _twitchDetectorService = twitchDetectorService;
            _configuration = configuration;
            GuildId = Convert.ToUInt32(_configuration["Discord:GuildId"]);
            ChannelId = Convert.ToUInt32(_configuration["Discord:ChannelId"]);
        }

        public async Task AnalyzeAsync()
        {
            var socketGuild = _client.GetGuild(GuildId);

            if (socketGuild is not null)
            {
                var streamUsers = await _twitchDetectorService.GetStreamingUsersAsync(socketGuild.Users.ToList());
                var streamManzibarUsers = await _twitchDetectorService.GetStreamingManzibarUsersAsync(streamUsers);
                var streamManzibarUsersForPing = await _twitchDetectorService.GetStreamingManzibarUsersToPingAsync(streamManzibarUsers);
                var embedBuilders = await EmbedBuilderService.GetEmbedBuilders(streamManzibarUsersForPing);

                if (embedBuilders.Count > 0)
                {
                    var socketTextChannel = socketGuild.GetTextChannel(ChannelId);

                    await socketTextChannel.SendMessageAsync("", false, embedBuilders.First().Build());
                }
            }
        }
    }
}