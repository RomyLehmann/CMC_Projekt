using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using CMC_Projekt.Services;

namespace CMC_Projekt
{
    /// <summary>
    /// Zentrale Datenverwaltung mit Backend-Synchronisation (500ms Polling)
    /// </summary>
    public static class BedDataManager
    {
        private static List<BettData> _lokaleBetten = new();
        private static readonly BettenApiService _apiService = new();
        private static DispatcherTimer? _pollTimer;
        private static bool _istInitialisiert = false;

        // Event für UI-Updates
        public static event Action? DatenAktualisiert;

        /// <summary>
        /// Initialisiert Backend-Verbindung und startet Polling (500ms)
        /// </summary>
        public static async Task InitializeAsync()
        {
            if (_istInitialisiert) return;

            Console.WriteLine("🔄 Backend-Verbindung wird initialisiert...");

            // Erste Datenladung
            await LadeVomBackendAsync();

            // Polling alle 500ms für Echtzeit-Updates
            _pollTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _pollTimer.Tick += async (s, e) => await LadeVomBackendAsync();
            _pollTimer.Start();

            _istInitialisiert = true;
            Console.WriteLine("✅ Backend-Verbindung aktiv (Polling: 500ms)");
        }

        /// <summary>
        /// Stoppt Polling (z.B. beim App-Schließen)
        /// </summary>
        public static void StopPolling()
        {
            _pollTimer?.Stop();
            Console.WriteLine("🛑 Polling gestoppt");
        }

        /// <summary>
        /// Lädt Daten vom Backend (wird automatisch alle 500ms aufgerufen)
        /// </summary>
        private static async Task LadeVomBackendAsync()
        {
            var betten = await _apiService.GetAlleBettenAsync();
            if (betten != null && betten.Any())
            {
                _lokaleBetten = betten;
                DatenAktualisiert?.Invoke();
            }
        }

        /// <summary>
        /// Alle Betten abrufen (aus lokalem Cache)
        /// </summary>
        public static List<BettData> GetAlleBetten()
        {
            return new List<BettData>(_lokaleBetten);
        }

        /// <summary>
        /// Einzelnes Bett finden
        /// </summary>
        public static BettData? GetBett(string bettNummer)
        {
            return _lokaleBetten.FirstOrDefault(b => b.BettNummer == bettNummer);
        }

        /// <summary>
        /// Bett aktualisieren (sendet zum Backend)
        /// </summary>
        public static async Task<bool> UpdateBettAsync(string bettNummer, string neuerStatus, string neueWartung)
        {
            bool erfolg = await _apiService.UpdateBettAsync(bettNummer, neuerStatus, neueWartung);

            if (erfolg)
            {
                // Sofort lokalen Cache aktualisieren (UI-Update innerhalb 500ms automatisch)
                var bett = _lokaleBetten.FirstOrDefault(b => b.BettNummer == bettNummer);
                if (bett != null)
                {
                    bett.Status = neuerStatus;
                    bett.Wartung = neueWartung;
                    bett.LetztGeaendert = DateTime.UtcNow;
                    DatenAktualisiert?.Invoke();
                }
            }

            return erfolg;
        }

        /// <summary>
        /// Neues Bett erstellen
        /// </summary>
        public static async Task<BettData?> CreateBettAsync(BettData bett)
        {
            var neuesBett = await _apiService.CreateBettAsync(bett);
            if (neuesBett != null)
            {
                _lokaleBetten.Add(neuesBett);
                DatenAktualisiert?.Invoke();
            }
            return neuesBett;
        }

        /// <summary>
        /// Bett löschen
        /// </summary>
        public static async Task<bool> DeleteBettAsync(string bettNummer)
        {
            bool erfolg = await _apiService.DeleteBettAsync(bettNummer);
            if (erfolg)
            {
                _lokaleBetten.RemoveAll(b => b.BettNummer == bettNummer);
                DatenAktualisiert?.Invoke();
            }
            return erfolg;
        }

        /// <summary>
        /// Manuelles Neuladen erzwingen
        /// </summary>
        public static async Task RefreshAsync()
        {
            await LadeVomBackendAsync();
        }
    }
}
