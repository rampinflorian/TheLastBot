using System.ComponentModel.DataAnnotations;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace TheLastBot.Server.Commands;

public class HelpModule : ModuleBase<SocketCommandContext>
{
    private readonly CommandService _service;

    public HelpModule(CommandService service)
    {
        _service = service;
    }

    [Command("help")]
    [Summary("Tu as besoin d'aide ? c'est bien !")]
    public async Task HelpAsync()
    {
        var builder = new ComponentBuilder()
            .WithButton("label", "custom-id");

        await ReplyAsync("Here is a button!", components: builder.Build());
    }
}