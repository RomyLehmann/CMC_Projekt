using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CMC_Projekt.Services
{
    public class BettenApiService
    {
        private readonly HttpClient _httpClient;
        private const string BASE_URL = "https://localhost:5001/api/Betten"; // ← PASSE DEINEN PORT AN!

        public BettenApiService()
        {
            // SSL-Zertifikat-Fehler in Entwicklung ignorieren
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BASE_URL),
                Timeout = TimeSpan.FromSeconds(5)
            };
        }

        // GET alle Betten
        public async Task<List<BettData>?> GetAlleBettenAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<BettData>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler beim Laden der Betten: {ex.Message}");
                return null;
            }
        }

        // GET einzelnes Bett
        public async Task<BettData?> GetBettAsync(string bettNummer)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/{bettNummer}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<BettData>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler beim Laden von Bett {bettNummer}: {ex.Message}");
                return null;
            }
        }

        // PUT Bett aktualisieren
        public async Task<bool> UpdateBettAsync(string bettNummer, string neuerStatus, string neueWartung)
        {
            try
            {
                var updateDto = new { Status = neuerStatus, Wartung = neueWartung };
                var response = await _httpClient.PutAsJsonAsync($"/{bettNummer}", updateDto);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Bett {bettNummer} aktualisiert: {neuerStatus}, {neueWartung}");
                    return true;
                }

                Console.WriteLine($"❌ Update fehlgeschlagen: {response.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler beim Update von Bett {bettNummer}: {ex.Message}");
                return false;
            }
        }

        // POST neues Bett erstellen
        public async Task<BettData?> CreateBettAsync(BettData bett)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("", bett);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<BettData>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler beim Erstellen von Bett: {ex.Message}");
                return null;
            }
        }

        // DELETE Bett löschen
        public async Task<bool> DeleteBettAsync(string bettNummer)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/{bettNummer}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler beim Löschen von Bett {bettNummer}: {ex.Message}");
                return false;
            }
        }
    }

    // DTO passend zum Backend
    public class BettData
    {
        public string BettNummer { get; set; } = string.Empty;
        public int ZimmerNummer { get; set; }
        public string Zimmer { get; set; } = string.Empty;
        public string Status { get; set; } = "Frei";
        public string Wartung { get; set; } = "Sauber";
        public DateTime LetztGeaendert { get; set; }
    }
}
