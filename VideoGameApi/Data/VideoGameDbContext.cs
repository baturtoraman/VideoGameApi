using Microsoft.EntityFrameworkCore;
using VideoGameApi.Entities;

namespace VideoGameApi.Data
{
    public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
    {
        public DbSet<VideoGame> VideoGames => Set<VideoGame>();
        public DbSet<VideoGameDetails> VideoGameDetails => Set<VideoGameDetails>();
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<VideoGame>().HasData(
                new VideoGame
                {
                    Id = 1,
                    Title = "Spid",
                    Platform = "PS5",
                    Price = 5,
                    Stock = 3
                },
                new VideoGame
                {
                    Id = 2,
                    Title = "Th",
                    Platform = "Nintendo Switch",
                    Price = 11,
                    Stock = 4
                },
                new VideoGame
                {
                    Id = 3,
                    Title = "Cyb",
                    Platform = "PC",
                    Price = 115,
                    Stock = 45
                }
            );
        }
    }

}
