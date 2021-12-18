using ManziBot.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ManziBot.Server.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<DiscordUser> DiscordUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        const string mySqlConnectionStr = @"server=localhost; port=3306; database=db_manzibot_dev; user=root; password=; Persist Security Info=False; Connect Timeout=300";
        options.UseMySql(mySqlConnectionStr, ServerVersion.AutoDetect(mySqlConnectionStr));
    }
}