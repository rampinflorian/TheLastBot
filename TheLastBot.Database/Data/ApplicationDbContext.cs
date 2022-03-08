using Microsoft.EntityFrameworkCore;
using TheLastBot.Database.Data.Models;

namespace TheLastBot.Database.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var mySqlConnectionStr = "";

#if DEBUG
        mySqlConnectionStr= @"Server=164.132.56.141;Initial Catalog=db_thelastbot_dev;User ID=SA;Password=Xdjsbrs2020!";

#elif RELEASE
        mySqlConnectionStr = Environment.GetEnvironmentVariable("TheLastBot_DATABASE_URL");

#endif

        options.UseMySql(mySqlConnectionStr, ServerVersion.AutoDetect(mySqlConnectionStr));
    }
    
    public DbSet<DiscordUser>? DiscordUsers { get; set; }
}