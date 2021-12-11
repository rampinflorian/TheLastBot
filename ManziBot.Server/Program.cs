using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ManziBot.Server.Data;
using ManziBot.Server.HandlerEvent;
using ManziBot.Server.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ManziBot.Server
{
    public class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                
                .AddScoped<CommandHandler>()
                .AddScoped<MessageHandler>()
                .AddScoped<LatencyUpdateHandler>()
                
                .AddScoped<StreamAnalyzeService>()
                .AddScoped<TwitchDetectorService>()
                .AddScoped<EmbedBuilderService>()
                .AddScoped<LogService>()
                .AddScoped<ApplicationDbContext>()
                
                .BuildServiceProvider();
            _client.Ready += () =>
            {
                Console.WriteLine("Bot is connected!");
                return Task.CompletedTask;
            };

            const string token = "OTEzMTM5MTAzOTcxODAzMjM3.YZ6JDw.QfWWIj5XUXUapg6fVuTWlUzK7TQ";

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await _services.GetRequiredService<CommandHandler>().InstallCommandAsync();
            await _services.GetRequiredService<MessageHandler>().InstallCommandAsync();
            await _services.GetRequiredService<LatencyUpdateHandler>().InstallCommandAsync();


            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        // private async Task ClientOnLatencyUpdated(int arg1, int arg2)
        // {
        //    await Log.Write(new LogMessage(LogSeverity.Debug, "", "Latence Updated"));
        //
        //     var guild = _client.GetGuild(_guildId);
        //
        //     if (guild is not null)
        //     {
        //         var channel = guild.GetTextChannel(_channelId);
        //         var streamAnalyseHandler = new StreamAnalyzeHandler();
        //         // streamAnalyseHandler.Initialize(_client, _commands, _services, channel, guild);
        //
        //         var guildUsers = channel.Users.ToList();
        //         await streamAnalyseHandler.AnalyzeAsync();
        //     }
        // }
    }
}