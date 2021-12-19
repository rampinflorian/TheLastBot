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

        private  ulong _guildId = 0;
        private  ulong _channelId = 0;

        public StreamAnalyzeService(DiscordSocketClient client, TwitchDetectorService twitchDetectorService)
        {
            _client = client;
            _twitchDetectorService = twitchDetectorService;
        }

        public async Task AnalyzeAsync()
        {
            var myIni = new IniFileService("Configuration.ini");
            
            _guildId = Convert.ToUInt64(myIni.Read("GuildId", "Discord"));
            _channelId = Convert.ToUInt64(myIni.Read("ChannelId", "Discord"));
            
            var socketGuild = _client.GetGuild(_guildId);

            if (socketGuild is not null)
            {
                var streamUsers = await _twitchDetectorService.GetStreamingUsersAsync(socketGuild.Users.ToList());
                var streamManzibarUsers = await _twitchDetectorService.GetStreamingManzibarUsersAsync(streamUsers);
                var streamManzibarUsersForPing = await _twitchDetectorService.GetStreamingManzibarUsersToPingAsync(streamManzibarUsers);
                var embedBuilders = await EmbedBuilderService.GetEmbedBuilders(streamManzibarUsersForPing);

                if (embedBuilders.Count > 0)
                {
                    var socketTextChannel = socketGuild.GetTextChannel(_channelId);

                    await socketTextChannel.SendMessageAsync("", false, embedBuilders.First().Build());
                }
            }
        }
    }
}