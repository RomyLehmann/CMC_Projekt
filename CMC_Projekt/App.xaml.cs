using System;
using System.Windows;

namespace CMC_Projekt
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Backend-Verbindung initialisieren (mit 500ms Polling)
                await BedDataManager.InitializeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Fehler beim Verbinden mit Backend:\n{ex.Message}\n\nApp startet trotzdem mit lokalen Daten.",
                    "Backend-Warnung",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Polling stoppen beim Beenden
            BedDataManager.StopPolling();
            base.OnExit(e);
        }
    }
}
