﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ManziBot.Server.Data;
using ManziBot.Server.Data.Dapper;
using ManziBot.Server.HandlerEvent;
using ManziBot.Server.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ManziBot.Server
{
    public class Program
    {
        private DiscordSocketClient? _client;
        private CommandService? _commands;
        private IServiceProvider? _services;


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
                
                .AddScoped<CustomQuery>()
                
                .BuildServiceProvider();
            _client.Ready += () =>
            {
                Console.WriteLine("Bot is connected!");
                return Task.CompletedTask;
            };
            
            var myIni = new IniFileService("Configuration.ini");
            var token = myIni.Read("ApiToken", "Discord"); 
            
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await _services.GetRequiredService<CommandHandler>().InstallCommandAsync();
            await _services.GetRequiredService<MessageHandler>().InstallCommandAsync();
            await _services.GetRequiredService<LatencyUpdateHandler>().InstallCommandAsync();


            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
    }
}