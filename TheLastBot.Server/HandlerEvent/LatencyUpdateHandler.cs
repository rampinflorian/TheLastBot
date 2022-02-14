using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TheLastBot.Server.Services;

namespace TheLastBot.Server.HandlerEvent;

public class LatencyUpdateHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly StreamAnalyzeService _streamAnalyzeService;


    public LatencyUpdateHandler(DiscordSocketClient client, CommandService commands, StreamAnalyzeService streamAnalyzeService)
    {
        _client = client;
        _commands = commands;
        _streamAnalyzeService = streamAnalyzeService;
    }

    public async Task InstallCommandAsync()
    {
        _client.LatencyUpdated += ClientOnLatencyUpdated;
        await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), 
            services: null);
    }

    private async Task ClientOnLatencyUpdated(int arg1, int arg2)
    {
        await LogService.Write(new LogMessage(LogSeverity.Debug, "", "Latence Updated"));
        await _streamAnalyzeService.AnalyzeAsync();
    }
}