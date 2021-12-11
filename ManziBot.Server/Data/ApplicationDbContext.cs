using ManziBot.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ManziBot.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<DiscordUser> StreamUsers { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer($"Server=164.132.56.141;Initial Catalog=db_manzibot_dev;User ID=SA;Password=B3z78xd+!");
    }
}