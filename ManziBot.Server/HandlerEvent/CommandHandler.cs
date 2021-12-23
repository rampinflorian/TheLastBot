using System.Reflection;
using Discord;
using Discord.Commands;

namespace ManziBot.Server.HandlerEvent
{
    public class CommandHandler
    {
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        public CommandHandler(CommandService commands, IServiceProvider services)
        {
            _commands = commands;
            _services = services;
            _commands.CommandExecuted += CommandExecutedAsync;
        }



        public async Task InstallCommandAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private static async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified)
            {
                Console.WriteLine(
                    $"Command failed to execute for [{context.User.Username}] <-> [{result.ErrorReason}]!");
                return;
            }

            if (result.IsSuccess)
            {
                Console.WriteLine($"Command [{command.Value.Name}] execute for [{context.User.Username}]!");
                return;
            }

            await context.Channel.SendMessageAsync($"{context.User.Username} Something went wrong -> [{result}]!");
        }
    }
}