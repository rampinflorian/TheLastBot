using Microsoft.EntityFrameworkCore;
using TheLastBot.Database.Data.Models;

namespace TheLastBot.Database.Data;

public class ApplicationDbContext : DbContext
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<DiscordUser>? DiscordUsers { get; set; }
}