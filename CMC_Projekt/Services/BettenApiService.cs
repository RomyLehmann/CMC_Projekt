using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CMC_Projekt.Services
{
    public class BettenApiService
    {
        private readonly HttpClient _httpClient;
        private const string BASE_URL = "https://localhost:5001/api/Betten";

        private static readonly HttpClient _sharedHttpClient;

        static BettenApiService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _sharedHttpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            _sharedHttpClient.BaseAddress = new Uri(BASE_URL);

            Console.WriteLine($"🌐 Shared HttpClient erstellt für {BASE_URL}");
        }

        public BettenApiService()
        {
            _httpClient = _sharedHttpClient;
        }

        // GET alle Betten
        public async Task<List<BettData>?> GetAlleBettenAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<BettData>>();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ GET Fehler: {ex.Message}");
                return null;
            }
        }

        // PUT Bett aktualisieren - VERBESSERT
        public async Task<bool> UpdateBettAsync(string bettNummer, string neuerStatus, string neueWartung)
        {
            try
            {
                var url = $"{BASE_URL}/{bettNummer}";
                var updateDto = new { Status = neuerStatus, Wartung = neueWartung };

                Console.WriteLine($"📤 PUT Request wird vorbereitet...");
                Console.WriteLine($"🔗 URL: {url}");
                Console.WriteLine($"📦 Daten: Status={neuerStatus}, Wartung={neueWartung}");

                var json = JsonSerializer.Serialize(updateDto);
                Console.WriteLine($"📄 JSON: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine($"🚀 Sende Request...");
                var response = await _httpClient.PutAsync(url, content);

                Console.WriteLine($"📊 Response empfangen: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"✅ UPDATE ERFOLGREICH!");
                    Console.WriteLine($"📄 Response: {responseBody}");
                    return true;
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ UPDATE FEHLGESCHLAGEN: {response.StatusCode}");
                    Console.WriteLine($"❌ Error: {errorBody}");
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ HTTP REQUEST EXCEPTION: {ex.Message}");
                Console.WriteLine($"❌ InnerException: {ex.InnerException?.Message}");
                return false;
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"❌ TIMEOUT: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ EXCEPTION: {ex.GetType().Name}");
                Console.WriteLine($"❌ Message: {ex.Message}");
                Console.WriteLine($"❌ Stack: {ex.StackTrace}");
                return false;
            }
        }

        // POST neues Bett
        public async Task<BettData?> CreateBettAsync(BettData bett)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("", bett);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<BettData>();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ POST Fehler: {ex.Message}");
                return null;
            }
        }

        // DELETE Bett
        public async Task<bool> DeleteBettAsync(string bettNummer)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/{bettNummer}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ DELETE Fehler: {ex.Message}");
                return false;
            }
        }
    }

    // DTO
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
