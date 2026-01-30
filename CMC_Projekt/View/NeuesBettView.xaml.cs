using CMC_Projekt;
using CMC_Projekt.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CMC_Projekt.View
{
    public partial class NeuesBettView : UserControl
    {
        // Optional: MainWindow kann das setzen, um nach Save automatisch zur Übersicht zu wechseln
        public Action? NavigateToUebersicht { get; set; }

        public NeuesBettView()
        {
            InitializeComponent();
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var zimmerNummerText = (txtZimmerNummer.Text ?? "").Trim();
            var bettNummer = (txtBettNummer.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(zimmerNummerText) || string.IsNullOrWhiteSpace(bettNummer))
            {
                MessageBox.Show("Bitte Zimmernummer und Bettnummer ausfüllen.", "Fehler",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(zimmerNummerText, out int zimmerNummer))
            {
                MessageBox.Show("Zimmernummer muss eine Zahl sein.", "Fehler",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var neuesBett = new BettData
                {
                    BettNummer = bettNummer,
                    ZimmerNummer = zimmerNummer,
                    Zimmer = zimmerNummer.ToString(),
                    Status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Frei",
                    Wartung = (cmbWartung.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Sauber"
                };

                var result = await BedDataManager.CreateBettAsync(neuesBett);

                if (result != null)
                {
                    txtInfo.Text = $"✅ Gespeichert: Bett {bettNummer}, Zimmer {zimmerNummer}";
                    txtZimmerNummer.Clear();
                    txtBettNummer.Clear();
                    cmbStatus.SelectedIndex = 0;
                    cmbWartung.SelectedIndex = 0;

                    // Nach 2 Sekunden zur Übersicht wechseln
                    await System.Threading.Tasks.Task.Delay(2000);
                    NavigateToUebersicht?.Invoke();
                }
                else
                {
                    MessageBox.Show("Fehler beim Speichern. Backend nicht erreichbar?", "Fehler",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Backend Error:\n{ex.Message}", "Fehler",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

