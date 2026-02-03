using Microsoft.EntityFrameworkCore;
using CMC_Backend.Models;

namespace CMC_Backend.Data
{
    public class BettenDbContext : DbContext
    {
        public BettenDbContext(DbContextOptions<BettenDbContext> options)
            : base(options)
        {
        }

        public DbSet<Bett> Betten { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed initial data - Alle 23 Betten aus deiner App
            modelBuilder.Entity<Bett>().HasData(
                new Bett { BettNummer = "B001", ZimmerNummer = 1, Zimmer = "1", Status = "Frei", Wartung = "Sauber" },
                new Bett { BettNummer = "B001-2", ZimmerNummer = 1, Zimmer = "1", Status = "Belegt", Wartung = "Sauber" },
                new Bett { BettNummer = "B002", ZimmerNummer = 2, Zimmer = "2", Status = "Belegt", Wartung = "Sauber" },
                new Bett { BettNummer = "B002-2", ZimmerNummer = 2, Zimmer = "2", Status = "Frei", Wartung = "Sauber" },
                new Bett { BettNummer = "B003", ZimmerNummer = 3, Zimmer = "3", Status = "Frei", Wartung = "Sauber" },
                new Bett { BettNummer = "B003-2", ZimmerNummer = 3, Zimmer = "3", Status = "Belegt", Wartung = "Nicht sauber" },
                new Bett { BettNummer = "B004", ZimmerNummer = 4, Zimmer = "4", Status = "Belegt", Wartung = "Sauber" },
                new Bett { BettNummer = "B005", ZimmerNummer = 5, Zimmer = "5", Status = "Frei", Wartung = "Sauber" },
                new Bett { BettNummer = "B005-2", ZimmerNummer = 5, Zimmer = "5", Status = "Belegt", Wartung = "Nicht sauber" },
                new Bett { BettNummer = "B006", ZimmerNummer = 6, Zimmer = "6", Status = "Belegt", Wartung = "Sauber" },
                new Bett { BettNummer = "B006-2", ZimmerNummer = 6, Zimmer = "6", Status = "Frei", Wartung = "Sauber" },
                new Bett { BettNummer = "B007", ZimmerNummer = 7, Zimmer = "7", Status = "Frei", Wartung = "Sauber" },
                new Bett { BettNummer = "B007-2", ZimmerNummer = 7, Zimmer = "7", Status = "Belegt", Wartung = "Sauber" },
                new Bett { BettNummer = "B008", ZimmerNummer = 8, Zimmer = "8", Status = "Belegt", Wartung = "Nicht sauber" },
                new Bett { BettNummer = "B008-2", ZimmerNummer = 8, Zimmer = "8", Status = "Frei", Wartung = "Sauber" },
                new Bett { BettNummer = "B009", ZimmerNummer = 9, Zimmer = "9", Status = "Frei", Wartung = "Sauber" },
                new Bett { BettNummer = "B009-2", ZimmerNummer = 9, Zimmer = "9", Status = "Belegt", Wartung = "Sauber" },
                new Bett { BettNummer = "B010", ZimmerNummer = 10, Zimmer = "10", Status = "Belegt", Wartung = "Sauber" },
                new Bett { BettNummer = "B010-2", ZimmerNummer = 10, Zimmer = "10", Status = "Frei", Wartung = "Sauber" },
                new Bett { BettNummer = "B011", ZimmerNummer = 11, Zimmer = "11", Status = "Frei", Wartung = "Sauber" },
                new Bett { BettNummer = "B011-2", ZimmerNummer = 11, Zimmer = "11", Status = "Belegt", Wartung = "Nicht sauber" },
                new Bett { BettNummer = "B012", ZimmerNummer = 12, Zimmer = "12", Status = "Belegt", Wartung = "Sauber" },
                new Bett { BettNummer = "B012-2", ZimmerNummer = 12, Zimmer = "12", Status = "Frei", Wartung = "Sauber" }
            );
        }
    }
}
