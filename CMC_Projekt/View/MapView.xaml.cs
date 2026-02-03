using CMC_Projekt.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CMC_Projekt.View
{
    public partial class MapView : UserControl
    {
        public MapView()
        {
            InitializeComponent();
        }

        private void Bett_Click(object sender, MouseButtonEventArgs e)
        {
            var ellipse = sender as System.Windows.Shapes.Ellipse;
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

            // Farben setzen
            txtStatus.Foreground = status == "Frei"
                ? new SolidColorBrush(Color.FromRgb(39, 174, 96))   // Grün
                : new SolidColorBrush(Color.FromRgb(231, 76, 60));  // Rot

            txtWartung.Foreground = wartung == "Sauber"
                ? new SolidColorBrush(Color.FromRgb(39, 174, 96))   // Grün
                : new SolidColorBrush(Color.FromRgb(230, 126, 34)); // Orange

            // Popup positionieren (in der Mitte des Bildschirms)
            var position = e.GetPosition(this);
            PopupTransform.X = position.X - 175; // Popup zentrieren (350px/2)
            PopupTransform.Y = position.Y - 125; // Popup zentrieren (250px/2)

            // Popup anzeigen
            BettInfoPopup.Visibility = Visibility.Visible;
        }

        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {
            BettInfoPopup.Visibility = Visibility.Collapsed;
        }
    }
}