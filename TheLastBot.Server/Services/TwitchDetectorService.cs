using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TheLastBot.Database.Data;
using TheLastBot.Database.Data.Dapper;
using TheLastBot.Database.Data.Models;

namespace TheLastBot.Server.Services
{
    public class TwitchDetectorService
    {
        private readonly ApplicationDbContext _context;
        private readonly CustomQuery _customQuery;
        private readonly IniFileService _iniFileService;

        public TwitchDetectorService(ApplicationDbContext context, CustomQuery customQuery)
        {
            _context = context;
            _customQuery = customQuery;
            _iniFileService = new IniFileService("Configuration.ini");

        }

        public static Task<List<SocketGuildUser>> GetStreamingUsersAsync(IEnumerable<SocketGuildUser> users)
        {
            var streamingUsers = new List<SocketGuildUser>();
            streamingUsers.AddRange(users.Where(m => !m.IsBot && m.Activities.Any(activity => activity.Name == "Twitch")));

            return Task.FromResult(streamingUsers);
        }

        public Task<List<SocketGuildUser>> GetStreamingTlhUsersAsync(List<SocketGuildUser> streamingUsers)
        {
            // var streamingTwitchUsers = streamingUsers.Where(streamingUser => streamingUser.Activities.Any(m => m.Name == "Twitch")).ToList();
            
            var TlhStreamingUsers = new List<SocketGuildUser>();

            foreach (var streamingTwitchUser in streamingUsers)
            {
                var twitchGame = (StreamingGame)streamingTwitchUser.Activities.FirstOrDefault(m => m.Name == "Twitch")!;
                if (IsTlhFlagExist(twitchGame.Details))
                {
                    TlhStreamingUsers.Add(streamingTwitchUser);
                }
            }
            
            return Task.FromResult(TlhStreamingUsers);
        }

        public async Task<List<SocketGuildUser>> GetStreamingTlhUsersToPingAsync(
            List<SocketGuildUser> streamTlhUsers)
        {
            var streamTlhUsersToPing = new List<SocketGuildUser>();

            var discordUsersInList = await _context.DiscordUsers
                .Where(m => streamTlhUsers.Select(s => s.Id).Contains(m.GuildUserId)).ToListAsync();

            foreach (var streamTlhUser in streamTlhUsers)
            {
                if (discordUsersInList.Any(m => m.GuildUserId == streamTlhUser.Id))
                {
                    // Si existe dans la liste
                    if (await _isStreamerReallyLaunchingStreamAsync(streamTlhUser))
                    {
                        streamTlhUsersToPing.Add(streamTlhUser);
                    }

                    await _updatedStreamInBaseAsync(streamTlhUser);
                }
                else
                {
                    // Si n'existe pas dans la liste
                    await _addNewStreamInBaseAsync(streamTlhUser);
                    streamTlhUsersToPing.Add(streamTlhUser);
                }
            }

            // Reset not streaming users
            await _resetNotStreamingUsersAsync(streamTlhUsers);
            
            return streamTlhUsersToPing;
        }

        private async Task _resetNotStreamingUsersAsync(List<SocketGuildUser> streamTlhUsers)
        {
            var streamTlhUsersArray = streamTlhUsers.Select(m => m.Id).ToArray();

            var sql = "UPDATE discordusers SET IsOnline = false WHERE GuildUserId NOT IN (@streamTlhUsersId)";
            var parameters = new { streamTlhUsersId = String.Join(',', streamTlhUsersArray) };
            try
            {
                await _customQuery.ExecuteAsync(sql, parameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task _addNewStreamInBaseAsync(SocketGuildUser streamTlhUser)
        {
            _context.Add(new DiscordUser
                {
                    IsOnline = true,
                    LastActivity = DateTime.Now,
                    GuildUserId = streamTlhUser.Id
                }
            );
            await _context.SaveChangesAsync();
        }

        private async Task _updatedStreamInBaseAsync(SocketGuildUser streamTlhUser)
        {


            const string sql = "UPDATE discordusers SET IsOnline = TRUE, LastActivity = NOW() WHERE GuildUserId = @streamTlhUserId";
            var parameters = new { streamTlhUserId = streamTlhUser.Id };
            try
            {
                await _customQuery.ExecuteAsync(sql, parameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task<bool> _isStreamerReallyLaunchingStreamAsync(SocketGuildUser streamTlhUser)
        {

            var discordUser = await _context.DiscordUsers.AsNoTracking().FirstAsync(m => m.GuildUserId == streamTlhUser.Id);
            var pingDelay = Convert.ToInt16(_iniFileService.Read("Delay", "Ping"));
            var result = discordUser.IsOnline == false && discordUser.LastActivity.AddMinutes(pingDelay) < DateTime.Now;
            
            return await Task.FromResult(result);
        }


        private static bool IsTlhFlagExist(string title)
        {
            var formatTitle = title.Replace(" ", String.Empty).ToUpper();
            var result = formatTitle.Contains("[THELASTHOPE]");
            return result;
        }
    }
}