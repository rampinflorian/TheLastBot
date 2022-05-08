using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TheLastBot.Server.HandlerEvent;
using TheLastBot.Server.Services;
using Microsoft.Extensions.DependencyInjection;
using TheLastBot.Database.Data;
using TheLastBot.Database.Data.Dapper;
using TheLastBot.Server.Commands;

namespace TheLastBot.Server
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
            var config = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.All
            };
            
            _client = new DiscordSocketClient(config);
            _commands = new CommandService();
            
            var mySqlConnectionStr = "";

#if DEBUG
            mySqlConnectionStr = "server=localhost; port=3306; database=db_thelastbot_dev; user=root; password=; Persist Security Info=False; Connect Timeout=300";
        
#elif RELEASE
            mySqlConnectionStr = Environment.GetEnvironmentVariable("DATABASE_URL");
#endif
            
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                
                
               
                .AddDbContext<ApplicationDbContext>(options => options.UseMySql(mySqlConnectionStr, ServerVersion.AutoDetect(mySqlConnectionStr)))

                .AddScoped<CommandHandler>()
                .AddScoped<MessageHandler>()
                .AddScoped<LatencyUpdateHandler>()
                
                .AddScoped<StreamAnalyzeService>()
                .AddScoped<TwitchDetectorService>()
                .AddScoped<EmbedBuilderService>()
                .AddScoped<LogService>()
                
                .AddScoped<HelpModule>()
                
                .AddScoped<CustomQuery>()
                
                .BuildServiceProvider();

            
#if DEBUG
            var myIni = new IniFileService("Configuration.ini");
            var token = myIni.Read("ApiToken", "Discord"); 
            #else
            var token = Environment.GetEnvironmentVariable("DISCORD_API_TOKEN");
            #endif

            await _services.GetRequiredService<CommandHandler>().InstallCommandAsync();
            await _services.GetRequiredService<MessageHandler>().InstallCommandAsync();
            await _services.GetRequiredService<LatencyUpdateHandler>().InstallCommandAsync();

            _client.Log += Log;
            
            _client.Ready += () =>
            {
                Console.WriteLine("Bot is connected!");
                return Task.CompletedTask;
            };
            
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
        
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}