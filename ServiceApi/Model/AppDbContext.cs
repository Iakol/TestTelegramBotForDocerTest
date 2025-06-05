using Microsoft.EntityFrameworkCore;

namespace ServiceApi.Model
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }


        public DbSet<UserModel> Users { get; set; }

        public DbSet<VibeModel> Vibes { get; set; }


    }
}
