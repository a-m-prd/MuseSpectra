using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MuseSpectra.Models;

namespace MuseSpectra.Data {
    public class AppDbContext : DbContext {
        public AppDbContext() : base("name=MuseSpectraDb") {

        }

        public DbSet<User> Users { get; set; }
    }
}
