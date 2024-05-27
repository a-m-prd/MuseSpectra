using MuseSpectra.Models; // Import the MuseSpectra.Models namespace for user model
using System.Data.Entity; // Import the Entity Framework namespace for database operations

namespace MuseSpectra.Data {
    public class AppDbContext : DbContext {
        // Initializes the DbContext with the connection string "MuseSpectraDb"
        public AppDbContext() : base("name=MuseSpectraDb") {

        }

        // DbSet representing the Users table in the database, containing User entities
        public DbSet<User> Users { get; set; }
    }
}