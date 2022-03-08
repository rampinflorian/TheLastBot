using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;

namespace TheLastBot.Server.HandlerEvent;

public class MessageHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;



    public MessageHandler(DiscordSocketClient client, CommandService commands)
    {
        _client = client;
        _commands = commands;
    }

    public async Task InstallCommandAsync()
    {
        _client.MessageReceived += HandleCommandAsync;
        _client.ButtonExecuted += MyButtonHandler;

        await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), 
            services: null);
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        // Don't process the command if it was a system message
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        // Create a number to track where the prefix ends and the command begins
        int argPos = 0;

        // Determine if the message is a command based on the prefix and make sure no bots trigger commands
        if (!(message.HasCharPrefix('!', ref argPos) ||
              message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
            return;

        // Create a WebSocket-based command context based on the message
        var context = new SocketCommandContext(_client, message);

        // Execute the command with the command context we just
        // created, along with the service provider for precondition checks.
        await _commands.ExecuteAsync(
            context: context,
            argPos: argPos,
            services: null);
    }
    
    private async Task MyButtonHandler(SocketMessageComponent component)
    {
        // We can now check for our custom id
        switch(component.Data.CustomId)
        {
            // Since we set our buttons custom id as 'custom-id', we can check for it like this:
            case "custom-id":
                // Lets respond by sending a message saying they clicked the button
                await component.RespondAsync($"{component.User.Mention} has clicked the button!");
                break;
        }
    }
    
}