using System.ComponentModel.DataAnnotations;

namespace TheLastBot.Database.Data.Models;

public class DiscordUser
{
    [Key]
    public int DiscordUserId { get; set; }
    public ulong GuildUserId { get; set; }
    public DateTime LastActivity { get; set; }
    public bool IsOnline { get; set; }
}