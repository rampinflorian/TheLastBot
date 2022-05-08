using Discord;
using Discord.WebSocket;

namespace TheLastBot.Server.Services
{
    public class StreamAnalyzeService
    {
        private readonly DiscordSocketClient _client;
        private readonly TwitchDetectorService _twitchDetectorService;

        private ulong _guildId;
        private ulong _channelId;

        public StreamAnalyzeService(DiscordSocketClient client, TwitchDetectorService twitchDetectorService)
        {
            _client = client;
            _twitchDetectorService = twitchDetectorService;
        }

        public async Task AnalyzeAsync()
        {
#if DEBUG
            var myIni = new IniFileService("Configuration.ini");

            _guildId = Convert.ToUInt64(myIni.Read("GuildId", "Discord"));
            _channelId = Convert.ToUInt64(myIni.Read("ChannelId", "Discord"));
#else
            _guildId =  Convert.ToUInt64(Environment.GetEnvironmentVariable("DISCORD_API_GUILDID"));
            _channelId = Convert.ToUInt64(Environment.GetEnvironmentVariable("DISCORD_API_CHANNELID"));
#endif
            var socketGuild = _client.GetGuild(_guildId);

            if (socketGuild is not null)
            {
                var streamUsers = await TwitchDetectorService.GetStreamingUsersAsync(socketGuild.Users.ToList());
                var streamTlhUsers = await _twitchDetectorService.GetStreamingTlhUsersAsync(streamUsers);
                var streamTlhUsersForPing =
                    await _twitchDetectorService.GetStreamingTlhUsersToPingAsync(streamTlhUsers);
                var embedBuilders = await EmbedBuilderService.GetEmbedBuilders(streamTlhUsersForPing);

                if (embedBuilders.Count > 0)
                {
                    var socketTextChannel = socketGuild.GetTextChannel(_channelId);

                    foreach (var embedBuilder in embedBuilders)
                    {
                        await socketTextChannel.SendMessageAsync("", false, embedBuilder.Build());
                    }

                    await LogService.Write(new LogMessage(LogSeverity.Debug, "",
                        "New streamer(s) : " + String.Join(", ", embedBuilders.Select(m => m.Author.Name))));
                }
            }
        }
    }
}