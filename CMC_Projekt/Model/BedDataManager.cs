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
        private static bool _backendErreichbar = false;

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
            Console.WriteLine($"✅ Backend-Verbindung aktiv (Polling: 500ms) - Backend erreichbar: {_backendErreichbar}");
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
            try
            {
                var betten = await _apiService.GetAlleBettenAsync();
                if (betten != null && betten.Any())
                {
                    bool hatteAenderungen = !_lokaleBetten.SequenceEqual(betten);
                    _lokaleBetten = betten;
                    _backendErreichbar = true;

                    // Event nur auslösen wenn es Änderungen gab
                    if (hatteAenderungen)
                    {
                        DatenAktualisiert?.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Backend nicht erreichbar: {ex.Message}");
                _backendErreichbar = false;
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
        /// Bett aktualisieren (sendet zum Backend) - VERBESSERT
        /// </summary>
        public static async Task<bool> UpdateBettAsync(string bettNummer, string neuerStatus, string neueWartung)
        {
            try
            {
                Console.WriteLine($"📤 Sende Update an Backend: {bettNummer} -> Status={neuerStatus}, Wartung={neueWartung}");

                bool erfolg = await _apiService.UpdateBettAsync(bettNummer, neuerStatus, neueWartung);

                if (erfolg)
                {
                    Console.WriteLine($"✅ Update erfolgreich: {bettNummer}");

                    // Sofort lokalen Cache aktualisieren
                    var bett = _lokaleBetten.FirstOrDefault(b => b.BettNummer == bettNummer);
                    if (bett != null)
                    {
                        bett.Status = neuerStatus;
                        bett.Wartung = neueWartung;
                        bett.LetztGeaendert = DateTime.UtcNow;
                    }

                    _backendErreichbar = true;
                    DatenAktualisiert?.Invoke();
                    return true;
                }
                else
                {
                    Console.WriteLine($"❌ Update fehlgeschlagen: {bettNummer}");
                    _backendErreichbar = false;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FEHLER beim Update: {ex.Message}");
                Console.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
                _backendErreichbar = false;
                return false;
            }
        }

        /// <summary>
        /// Neues Bett erstellen
        /// </summary>
        public static async Task<BettData?> CreateBettAsync(BettData bett)
        {
            try
            {
                Console.WriteLine($"📤 Erstelle neues Bett: {bett.BettNummer}");

                var neuesBett = await _apiService.CreateBettAsync(bett);
                if (neuesBett != null)
                {
                    Console.WriteLine($"✅ Bett erstellt: {neuesBett.BettNummer}");
                    _lokaleBetten.Add(neuesBett);
                    _backendErreichbar = true;
                    DatenAktualisiert?.Invoke();
                }
                else
                {
                    Console.WriteLine($"❌ Bett konnte nicht erstellt werden");
                    _backendErreichbar = false;
                }
                return neuesBett;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FEHLER beim Erstellen: {ex.Message}");
                _backendErreichbar = false;
                return null;
            }
        }

        /// <summary>
        /// Bett löschen
        /// </summary>
        public static async Task<bool> DeleteBettAsync(string bettNummer)
        {
            try
            {
                bool erfolg = await _apiService.DeleteBettAsync(bettNummer);
                if (erfolg)
                {
                    _lokaleBetten.RemoveAll(b => b.BettNummer == bettNummer);
                    _backendErreichbar = true;
                    DatenAktualisiert?.Invoke();
                }
                else
                {
                    _backendErreichbar = false;
                }
                return erfolg;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler beim Löschen: {ex.Message}");
                _backendErreichbar = false;
                return false;
            }
        }

        /// <summary>
        /// Manuelles Neuladen erzwingen
        /// </summary>
        public static async Task RefreshAsync()
        {
            await LadeVomBackendAsync();
        }

        /// <summary>
        /// Prüft ob Backend erreichbar ist
        /// </summary>
        public static bool IstBackendErreichbar()
        {
            return _backendErreichbar;
        }
    }
}