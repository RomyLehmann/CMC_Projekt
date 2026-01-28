using System.Collections.Generic;
using System.Linq;

namespace CMC_Projekt
{
    // Zentrale Datenverwaltung für alle Betten
    public static class BedDataManager
    {
        private static List<BettData> betten;

        static BedDataManager()
        {
            InitializeBetten();
        }

        private static void InitializeBetten()
        {
            betten = new List<BettData>
            {
                new BettData { ZimmerNummer = 1, Zimmer = "1", BettNummer = "B001", Status = "Frei", Wartung = "Sauber" },
                new BettData { ZimmerNummer = 1, Zimmer = "1", BettNummer = "B001-2", Status = "Belegt", Wartung = "Sauber" },
                new BettData { ZimmerNummer = 2, Zimmer = "2", BettNummer = "B002", Status = "Belegt", Wartung = "Sauber" },
                new BettData { ZimmerNummer = 2, Zimmer = "2", BettNummer = "B002-2", Status = "Frei", Wartung = "Sauber" },
                new BettData { ZimmerNummer = 3, Zimmer = "3", BettNummer = "B003", Status = "Frei", Wartung = "Sauber" },
                new BettData { ZimmerNummer = 3, Zimmer = "3", BettNummer = "B003-2", Status = "Belegt", Wartung = "Nicht sauber" },
                new BettData { ZimmerNummer = 4, Zimmer = "4", BettNummer = "B004", Status = "Belegt", Wartung = "Sauber" },
                new BettData { ZimmerNummer = 5, Zimmer = "5", BettNummer = "B005", Status = "Frei", Wartung = "Sauber" },
                new BettData { ZimmerNummer = 5, Zimmer = "5", BettNummer = "B005-2", Status = "Belegt", Wartung = "Nicht sauber" },
                new BettData { ZimmerNummer = 6, Zimmer = "6", BettNummer = "B006", Status = "Belegt", Wartung = "Sauber" },
                new BettData { ZimmerNummer = 6, Zimmer = "6", BettNummer = "B006-2", Status = "Frei", Wartung = "Sauber" },
                new BettData { ZimmerNummer = 7, Zimmer = "7", BettNummer = "B007", Status = "Frei", Wartung = "Sauber" },
                new BettData { ZimmerNummer = 7, Zimmer = "7", BettNummer = "B007-2", Status = "Belegt", Wartung = "Sauber" },
                new BettData { ZimmerNummer = 8, Zimmer = "8", BettNummer = "B008", Status = "Belegt", Wartung = "Nicht sauber" },
                new BettData { ZimmerNummer = 8, Zimmer = "8", BettNummer = "B008-2", Status = "Frei", Wartung = "Sauber" },
                new BettData { ZimmerNummer = 9, Zimmer = "9", BettNummer = "B009", Status = "Frei", Wartung = "Sauber" },
                new BettData { ZimmerNummer = 9, Zimmer = "9", BettNummer = "B009-2", Status = "Belegt", Wartung = "Sauber" },
                new BettData { ZimmerNummer = 10, Zimmer = "10", BettNummer = "B010", Status = "Belegt", Wartung = "Sauber" },
                new BettData { ZimmerNummer = 10, Zimmer = "10", BettNummer = "B010-2", Status = "Frei", Wartung = "Sauber" },
                new BettData { ZimmerNummer = 11, Zimmer = "11", BettNummer = "B011", Status = "Frei", Wartung = "Sauber" },
                new BettData { ZimmerNummer = 11, Zimmer = "11", BettNummer = "B011-2", Status = "Belegt", Wartung = "Nicht sauber" },
                new BettData { ZimmerNummer = 12, Zimmer = "12", BettNummer = "B012", Status = "Belegt", Wartung = "Sauber" },
                new BettData { ZimmerNummer = 12, Zimmer = "12", BettNummer = "B012-2", Status = "Frei", Wartung = "Sauber" }
            };
        }

        // Alle Betten abrufen
        public static List<BettData> GetAlleBetten()
        {
            return new List<BettData>(betten);
        }

        // Ein bestimmtes Bett finden
        public static BettData GetBett(string bettNummer)
        {
            return betten.FirstOrDefault(b => b.BettNummer == bettNummer);
        }

        // Bett aktualisieren
        public static bool UpdateBett(string bettNummer, string neuerStatus, string neueWartung)
        {
            var bett = betten.FirstOrDefault(b => b.BettNummer == bettNummer);
            if (bett != null)
            {
                bett.Status = neuerStatus;
                bett.Wartung = neueWartung;
                return true;
            }
            return false;
        }

        // Daten zurücksetzen (für Tests)
        public static void Reset()
        {
            InitializeBetten();
        }
    }

    // Datenmodell für Betten
    public class BettData
    {
        public int ZimmerNummer { get; set; }
        public string Zimmer { get; set; }
        public string BettNummer { get; set; }
        public string Status { get; set; }
        public string Wartung { get; set; }
    }
}
