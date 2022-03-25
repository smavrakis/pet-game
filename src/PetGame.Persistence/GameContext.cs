using Microsoft.EntityFrameworkCore;
using PetGame.Persistence.Models;

namespace PetGame.Persistence
{
    public class GameContext : DbContext
    {
        public GameContext(DbContextOptions<GameContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players => Set<Player>();
        public DbSet<Pet> Pets => Set<Pet>();
    }
}
