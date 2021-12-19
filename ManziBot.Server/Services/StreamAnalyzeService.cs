using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace ManziBot.Server.Services
{
    public class StreamAnalyzeService
    {
        private readonly DiscordSocketClient _client;
        private readonly TwitchDetectorService _twitchDetectorService;

        private const ulong GuildId = 913137817230659665;
        private const ulong ChannelId = 913137817230659668;

        public StreamAnalyzeService(DiscordSocketClient client, TwitchDetectorService twitchDetectorService)
        {
            _client = client;
            _twitchDetectorService = twitchDetectorService;
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