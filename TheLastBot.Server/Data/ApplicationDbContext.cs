using TheLastBot.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace TheLastBot.Server.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<DiscordUser> DiscordUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var mySqlConnectionStr = "";

#if DEBUG
        mySqlConnectionStr= @"server=localhost; port=3306; database=db_TheLastBot_dev; user=root; password=; Persist Security Info=False; Connect Timeout=300";

#elif RELEASE
        mySqlConnectionStr = Environment.GetEnvironmentVariable("TheLastBot_DATABASE_URL");

#endif

        options.UseMySql(mySqlConnectionStr, ServerVersion.AutoDetect(mySqlConnectionStr));
    }
}