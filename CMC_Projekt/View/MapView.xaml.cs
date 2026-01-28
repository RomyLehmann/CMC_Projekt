using CMC_Projekt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CMC_Projekt.View
{
    public partial class MapView : UserControl
    {
        private Ellipse currentlySelectedPin = null;
        private Dictionary<string, Ellipse> bettPunkte;

        public MapView()
        {
            InitializeComponent();
            bettPunkte = new Dictionary<string, Ellipse>();
            this.Loaded += MapView_Loaded;
        }

        private void MapView_Loaded(object sender, RoutedEventArgs e)
        {
            // Alle Bett-Punkte sammeln und Farben aktualisieren
            SammleBettPunkteUndAktualisiere();
        }

        private void SammleBettPunkteUndAktualisiere()
        {
            // Canvas finden
            var canvas = FindVisualChild<Canvas>(this);
            if (canvas != null)
            {
                // Alle Ellipsen mit Tag durchgehen
                foreach (var child in canvas.Children)
                {
                    if (child is Ellipse ellipse && ellipse.Tag != null)
                    {
                        string tag = ellipse.Tag.ToString();
                        var parts = tag.Split('|');
                        if (parts.Length >= 2)
                        {
                            string bettNummer = parts[1]; // z.B. "B001"
                            bettPunkte[bettNummer] = ellipse;
                        }
                    }
                }

                // Farben aktualisieren
                AktualisiereKarte();
            }
        }

        public void AktualisiereKarte()
        {
            var alleBetten = BedDataManager.GetAlleBetten();

            foreach (var bett in alleBetten)
            {
                if (bettPunkte.ContainsKey(bett.BettNummer))
                {
                    // Farbe basierend auf Status setzen
                    bettPunkte[bett.BettNummer].Fill = bett.Status == "Frei" ? Brushes.Green : Brushes.Red;
                }
            }
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                {
                    return typedChild;
                }
                var result = FindVisualChild<T>(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private void Bett_Click(object sender, MouseButtonEventArgs e)
        {
            // Pinnadel wurde geklickt
            if (sender is Ellipse ellipse && ellipse.Tag != null)
            {
                // Wenn derselbe Pin erneut geklickt wird, Popup schließen
                if (currentlySelectedPin == ellipse && BettInfoPopup.Visibility == Visibility.Visible)
                {
                    BettInfoPopup.Visibility = Visibility.Collapsed;
                    currentlySelectedPin = null;
                    return;
                }

                // Aktuellen Pin merken
                currentlySelectedPin = ellipse;

                // Tag Format: "Zimmernummer|Bettnummer|Status|Wartung"
                string[] info = ellipse.Tag.ToString().Split('|');

                if (info.Length >= 2)
                {
                    string bettNummer = info[1];

                    // Aktuelle Daten aus BettDataManager holen
                    var bett = BedDataManager.GetBett(bettNummer);
                    if (bett != null)
                    {
                        txtZimmer.Text = "Zimmer: " + bett.Zimmer;
                        txtBettNummer.Text = "Bett: " + bett.BettNummer;
                        txtStatus.Text = bett.Status;
                        txtWartung.Text = bett.Wartung;
                    }
                    else
                    {
                        // Fallback zu Tag-Daten
                        if (info.Length == 4)
                        {
                            txtZimmer.Text = "Zimmer: " + info[0];
                            txtBettNummer.Text = "Bett: " + info[1];
                            txtStatus.Text = info[2];
                            txtWartung.Text = info[3];
                        }
                    }

                    // Position des Pins ermitteln
                    Point pinPosition = ellipse.TransformToAncestor(this).Transform(new Point(0, 0));

                    // Popup oberhalb des Pins positionieren
                    double popupX = pinPosition.X - (BettInfoPopup.Width / 2) + (ellipse.Width / 2);
                    double popupY = pinPosition.Y - BettInfoPopup.Height - 10;

                    // Sicherstellen, dass Popup nicht aus dem Fenster ragt (links)
                    if (popupX < 0)
                    {
                        popupX = 10;
                    }

                    // Sicherstellen, dass Popup nicht aus dem Fenster ragt (rechts)
                    if (popupX + BettInfoPopup.Width > this.ActualWidth)
                    {
                        popupX = this.ActualWidth - BettInfoPopup.Width - 10;
                    }

                    // Wenn nicht genug Platz oberhalb, unterhalb anzeigen
                    if (popupY < 0)
                    {
                        popupY = pinPosition.Y + ellipse.Height + 10;
                    }

                    // Position über RenderTransform setzen
                    PopupTransform.X = popupX;
                    PopupTransform.Y = popupY;

                    // Popup anzeigen
                    BettInfoPopup.Visibility = Visibility.Visible;
                }
            }
        }

        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {
            // Popup schließen
            BettInfoPopup.Visibility = Visibility.Collapsed;
            currentlySelectedPin = null;
        }
    }
}
