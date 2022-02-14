using System.ComponentModel.DataAnnotations;
using Discord;
using Discord.Commands;
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
    public async Task HelpAsync(string echo)
    {
        
        await ReplyAsync($"Tu as dit : {echo}");
    }
}