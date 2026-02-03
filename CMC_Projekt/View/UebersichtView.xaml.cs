using CMC_Projekt;
using CMC_Projekt.View;
using System.Collections.Generic;
using System.Linq;
using CMC_Projekt.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CMC_Projekt.View
{
    public partial class UebersichtView : UserControl
    {
        private List<BettData> aktuelleListe;
        private bool istGefiltert = false;
        private string letzterSuchbegriff = string.Empty;

        private bool zimmerAufsteigend = true;
        private bool bettAufsteigend = true;
        private bool statusAufsteigend = true;
        private bool wartungAufsteigend = true;

        public UebersichtView()
        {
            InitializeComponent();
            LadeBettenDaten();

            // Event registrieren
            BedDataManager.DatenAktualisiert += OnDatenAktualisiert;
        }

        private void OnDatenAktualisiert()
        {
            Dispatcher.Invoke(() =>
            {
                // NUR neu laden wenn KEIN Filter aktiv
                if (!istGefiltert)
                {
                    LadeBettenDaten();
                }
            });
        }

        private void LadeBettenDaten()
        {
            var alleBetten = BedDataManager.GetAlleBetten();
            aktuelleListe = new List<BettData>(alleBetten);

            BettenDataGrid.ItemsSource = null;
            BettenDataGrid.ItemsSource = aktuelleListe;
        }

        public void RefreshData()
        {
            if (!istGefiltert)
            {
                LadeBettenDaten();
            }
        }

        private void AendernButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null)
            {
                string bettNummer = button.Tag.ToString();

                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    var aenderungenView = new AenderungenView(bettNummer);
                    mainWindow.MainContent.Content = aenderungenView;
                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SucheDurchfuehren();
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SucheDurchfuehren();
            }
        }

        private void SucheDurchfuehren()
        {
            string suchbegriff = SearchTextBox.Text.Trim().ToLower();
            letzterSuchbegriff = suchbegriff;

            if (string.IsNullOrEmpty(suchbegriff))
            {
                istGefiltert = false;
                LadeBettenDaten();
                return;
            }

            // Filter AN
            istGefiltert = true;

            var alleBetten = BedDataManager.GetAlleBetten();
            var gefiltert = alleBetten.Where(b =>
                b.Zimmer.ToLower().Contains(suchbegriff) ||
                b.BettNummer.ToLower().Contains(suchbegriff) ||
                b.Status.ToLower().Contains(suchbegriff) ||
                b.Wartung.ToLower().Contains(suchbegriff)
            ).ToList();

            aktuelleListe = gefiltert;
            BettenDataGrid.ItemsSource = null;
            BettenDataGrid.ItemsSource = aktuelleListe;

            if (gefiltert.Count == 0)
            {
                MessageBox.Show("Keine Betten gefunden.", "Suche",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Clear();
            letzterSuchbegriff = string.Empty;

            // Filter AUS
            istGefiltert = false;

            // Reset
            zimmerAufsteigend = true;
            bettAufsteigend = true;
            statusAufsteigend = true;
            wartungAufsteigend = true;

            LadeBettenDaten();
        }

        private void SortZimmer_Click(object sender, RoutedEventArgs e)
        {
            istGefiltert = true;

            if (zimmerAufsteigend)
            {
                aktuelleListe = aktuelleListe.OrderBy(b => b.ZimmerNummer).ToList();
            }
            else
            {
                aktuelleListe = aktuelleListe.OrderByDescending(b => b.ZimmerNummer).ToList();
            }
            zimmerAufsteigend = !zimmerAufsteigend;

            BettenDataGrid.ItemsSource = null;
            BettenDataGrid.ItemsSource = aktuelleListe;
        }

        private void SortBett_Click(object sender, RoutedEventArgs e)
        {
            istGefiltert = true;

            if (bettAufsteigend)
            {
                aktuelleListe = aktuelleListe.OrderBy(b => b.BettNummer).ToList();
            }
            else
            {
                aktuelleListe = aktuelleListe.OrderByDescending(b => b.BettNummer).ToList();
            }
            bettAufsteigend = !bettAufsteigend;

            BettenDataGrid.ItemsSource = null;
            BettenDataGrid.ItemsSource = aktuelleListe;
        }

        private void SortStatus_Click(object sender, RoutedEventArgs e)
        {
            istGefiltert = true;

            if (statusAufsteigend)
            {
                aktuelleListe = aktuelleListe.OrderBy(b => b.Status).ToList();
            }
            else
            {
                aktuelleListe = aktuelleListe.OrderByDescending(b => b.Status).ToList();
            }
            statusAufsteigend = !statusAufsteigend;

            BettenDataGrid.ItemsSource = null;
            BettenDataGrid.ItemsSource = aktuelleListe;
        }

        private void SortWartung_Click(object sender, RoutedEventArgs e)
        {
            istGefiltert = true;

            if (wartungAufsteigend)
            {
                aktuelleListe = aktuelleListe.OrderBy(b => b.Wartung).ToList();
            }
            else
            {
                aktuelleListe = aktuelleListe.OrderByDescending(b => b.Wartung).ToList();
            }
            wartungAufsteigend = !wartungAufsteigend;

            BettenDataGrid.ItemsSource = null;
            BettenDataGrid.ItemsSource = aktuelleListe;
        }
    }
}