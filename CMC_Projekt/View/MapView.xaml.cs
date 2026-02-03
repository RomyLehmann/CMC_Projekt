using CMC_Projekt.Services;
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
        // Dictionary: BettNummer -> Ellipse-Objekt
        private Dictionary<string, Ellipse> bettEllipsenMap = new Dictionary<string, Ellipse>();

        public MapView()
        {
            InitializeComponent();

            // Alle Ellipsen im XAML finden und in Dictionary speichern
            Loaded += MapView_Loaded;

            // Event für Auto-Updates registrieren
            BedDataManager.DatenAktualisiert += OnDatenAktualisiert;
        }

        /// <summary>
        /// Wird aufgerufen wenn die View geladen ist
        /// </summary>
        private void MapView_Loaded(object sender, RoutedEventArgs e)
        {
            // Alle Ellipsen im Canvas finden und zuordnen
            SammleBettEllipsen();

            // Initiale Farben und Tags aus Backend setzen
            AktualisiereMapDaten();
        }

        /// <summary>
        /// Event-Handler: Wird aufgerufen wenn Backend-Daten sich ändern
        /// </summary>
        private void OnDatenAktualisiert()
        {
            Dispatcher.Invoke(() =>
            {
                AktualisiereMapDaten();
            });
        }

        /// <summary>
        /// Findet alle Ellipsen im Canvas und speichert sie im Dictionary
        /// </summary>
        private void SammleBettEllipsen()
        {
            bettEllipsenMap.Clear();

            // Canvas im Border finden
            var border = this.Content as Grid;
            if (border == null) return;

            var scrollViewer = FindChild<ScrollViewer>(border);
            if (scrollViewer == null) return;

            var mainBorder = scrollViewer.Content as Border;
            if (mainBorder == null) return;

            var canvas = mainBorder.Child as Canvas;
            if (canvas == null) return;

            // Alle Ellipsen durchgehen
            foreach (var child in canvas.Children)
            {
                if (child is Ellipse ellipse && ellipse.Tag != null)
                {
                    string tag = ellipse.Tag.ToString();
                    string[] parts = tag.Split('|');

                    if (parts.Length >= 2)
                    {
                        string bettNummer = parts[1]; // z.B. "B001"

                        if (!bettEllipsenMap.ContainsKey(bettNummer))
                        {
                            bettEllipsenMap[bettNummer] = ellipse;
                        }
                    }
                }
            }

            Console.WriteLine($"🗺️ {bettEllipsenMap.Count} Bett-Punkte gefunden im Layout");
        }

        /// <summary>
        /// Aktualisiert Farben und Tags aller Betten basierend auf Backend-Daten
        /// </summary>
        private void AktualisiereMapDaten()
        {
            var alleBetten = BedDataManager.GetAlleBetten();
            if (alleBetten == null || !alleBetten.Any())
            {
                Console.WriteLine("⚠️ Keine Backend-Daten verfügbar");
                return;
            }

            int aktualisiert = 0;

            foreach (var bett in alleBetten)
            {
                if (bettEllipsenMap.ContainsKey(bett.BettNummer))
                {
                    var ellipse = bettEllipsenMap[bett.BettNummer];

                    // Farbe basierend auf Status setzen
                    SetBettFarbe(ellipse, bett.Status);

                    // Tag aktualisieren (für Popup-Daten)
                    ellipse.Tag = $"{bett.Zimmer}|{bett.BettNummer}|{bett.Status}|{bett.Wartung}";

                    // Tooltip aktualisieren
                    ellipse.ToolTip = $"{bett.BettNummer}\n{bett.Status}\n{bett.Wartung}";

                    aktualisiert++;
                }
            }

            Console.WriteLine($"🗺️ MapView aktualisiert: {aktualisiert} Betten");
        }

        /// <summary>
        /// Setzt Farbe der Ellipse basierend auf Status
        /// Frei = Grün, Belegt = Rot (unabhängig von Wartungszustand)
        /// </summary>
        private void SetBettFarbe(Ellipse ellipse, string status)
        {
            if (status == "Frei")
            {
                // Grün für Frei
                ellipse.Fill = new SolidColorBrush(Color.FromRgb(0, 128, 0)); // Green
                ellipse.Stroke = new SolidColorBrush(Color.FromRgb(0, 100, 0)); // DarkGreen
            }
            else if (status == "Belegt")
            {
                // Rot für Belegt
                ellipse.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0)); // Red
                ellipse.Stroke = new SolidColorBrush(Color.FromRgb(139, 0, 0)); // DarkRed
            }
            else
            {
                // Fallback für unbekannte Status
                ellipse.Fill = Brushes.Gray;
                ellipse.Stroke = Brushes.DarkGray;
            }
        }

        /// <summary>
        /// Wird aufgerufen wenn auf einen Bett-Punkt geklickt wird
        /// </summary>
        private void Bett_Click(object sender, MouseButtonEventArgs e)
        {
            var ellipse = sender as Ellipse;
            if (ellipse?.Tag == null) return;

            string[] data = ellipse.Tag.ToString().Split('|');
            if (data.Length < 4) return;

            string zimmer = data[0];
            string bettNummer = data[1];
            string status = data[2];
            string wartung = data[3];

            // Popup-Daten setzen
            txtBettNummer.Text = $"Bett: {bettNummer}";
            txtZimmer.Text = $"Zimmer: {zimmer}";
            txtStatus.Text = status;
            txtWartung.Text = wartung;

            // Farben für Status setzen
            if (status == "Frei")
            {
                txtStatus.Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96)); // Grün
            }
            else if (status == "Belegt")
            {
                txtStatus.Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60)); // Rot
            }
            else
            {
                txtStatus.Foreground = Brushes.Gray;
            }

            // Farben für Wartung setzen
            if (wartung == "Sauber")
            {
                txtWartung.Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96)); // Grün
            }
            else if (wartung == "Nicht sauber")
            {
                txtWartung.Foreground = new SolidColorBrush(Color.FromRgb(230, 126, 34)); // Orange
            }
            else
            {
                txtWartung.Foreground = Brushes.Gray;
            }

            // Popup-Position berechnen (zentriert um Mausposition)
            var position = e.GetPosition(this);

            // Popup 300x220, also Offset -150 / -110 für Zentrierung
            double offsetX = position.X - 150;
            double offsetY = position.Y - 110;

            // Verhindere dass Popup außerhalb des Fensters ist
            double maxX = this.ActualWidth - 310; // 300 + 10 Margin
            double maxY = this.ActualHeight - 230; // 220 + 10 Margin

            offsetX = Math.Max(10, Math.Min(offsetX, maxX));
            offsetY = Math.Max(10, Math.Min(offsetY, maxY));

            PopupTransform.X = offsetX;
            PopupTransform.Y = offsetY;

            // Popup anzeigen
            BettInfoPopup.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Schließt das Popup
        /// </summary>
        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {
            BettInfoPopup.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Hilfsmethode: Findet Child-Element im Visual Tree
        /// </summary>
        private T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T result)
                {
                    return result;
                }

                var childOfChild = FindChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }

            return null;
        }
    }
}
