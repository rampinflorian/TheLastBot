using Discord.WebSocket;
using ManziBot.Server.Data;
using ManziBot.Server.Data.Dapper;
using ManziBot.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ManziBot.Server.Services
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

        public Task<List<SocketGuildUser>> GetStreamingUsersAsync(List<SocketGuildUser> users)
        {
            var streamingUsers = new List<SocketGuildUser>();
            streamingUsers.AddRange(users.Where(m => !m.IsBot && m.Activity is not null));

            return Task.FromResult(streamingUsers);
        }

        public Task<List<SocketGuildUser>> GetStreamingManzibarUsersAsync(List<SocketGuildUser> streamingUsers)
        {
            var manzibarStreamingUsers = streamingUsers
                .Where(streamingUser => IsManzibarFlagExist(streamingUser.Activity.Details)).ToList();
            return Task.FromResult(manzibarStreamingUsers);
        }

        public async Task<List<SocketGuildUser>> GetStreamingManzibarUsersToPingAsync(
            List<SocketGuildUser> streamManzibarUsers)
        {
            var streamManzibarUsersToPing = new List<SocketGuildUser>();

            var id = streamManzibarUsers.Select(m => m.Id).ToArray();
            var discordUsersInList = await _context.DiscordUsers
                .Where(m => streamManzibarUsers.Select(s => s.Id).Contains(m.GuildUserId)).ToListAsync();

            foreach (var streamManzibarUser in streamManzibarUsers)
            {
                if (discordUsersInList.Any(m => m.GuildUserId == streamManzibarUser.Id))
                {
                    // Si existe dans la liste
                    if (await _isStreamerReallyLaunchingStreamAsync(streamManzibarUser))
                    {
                        streamManzibarUsersToPing.Add(streamManzibarUser);
                    }

                    await _updatedStreamInBaseAsync(streamManzibarUser);
                }
                else
                {
                    // Si n'existe pas dans la liste
                    await _addNewStreamInBaseAsync(streamManzibarUser);
                    streamManzibarUsersToPing.Add(streamManzibarUser);
                }
            }

            // Reset not streaming users
            await _resetNotStreamingUsersAsync(streamManzibarUsers);
            
            return streamManzibarUsersToPing;
        }

        private async Task _resetNotStreamingUsersAsync(List<SocketGuildUser> streamManzibarUsers)
        {
            var streamManzibarUsersArray = streamManzibarUsers.Select(m => m.Id).ToArray();

            var sql = "UPDATE discordusers SET IsOnline = false WHERE GuildUserId NOT IN (@streamManzibarUsersId)";
            var parameters = new { streamManzibarUsersId = String.Join(',', streamManzibarUsersArray) };
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

        private async Task _addNewStreamInBaseAsync(SocketGuildUser streamManzibarUser)
        {
            _context.Add(new DiscordUser
                {
                    IsOnline = true,
                    LastActivity = DateTime.Now,
                    GuildUserId = streamManzibarUser.Id
                }
            );
            await _context.SaveChangesAsync();
        }

        private async Task _updatedStreamInBaseAsync(SocketGuildUser streamManzibarUser)
        {


            var sql = "UPDATE discordusers SET IsOnline = TRUE, LastActivity = NOW() WHERE GuildUserId = @streamManzibarUserId";
            var parameters = new { streamManzibarUserId = streamManzibarUser.Id };
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

        private async Task<bool> _isStreamerReallyLaunchingStreamAsync(SocketGuildUser streamManzibarUser)
        {

            var discordUser = await _context.DiscordUsers.AsNoTracking().FirstAsync(m => m.GuildUserId == streamManzibarUser.Id);
            var pingDelay = Convert.ToInt16(_iniFileService.Read("Delay", "Ping"));
            var result = discordUser.IsOnline == false && discordUser.LastActivity.AddMinutes(pingDelay) < DateTime.Now;
            
            return await Task.FromResult(result);
        }


        private static bool IsManzibarFlagExist(string title)
        {
            var formatTitle = title.Replace(" ", String.Empty).ToUpper();
            var result = formatTitle.Contains("[MANZIBAR]");
            return result;
        }
    }
}