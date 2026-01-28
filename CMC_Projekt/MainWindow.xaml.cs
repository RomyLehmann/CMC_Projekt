using CMC_Projekt.View;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace CMC_Projekt
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            // Startseite laden (Stationskarte)
            MainContent.Content = new MapView();
        }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            // Menü ein-/ausblenden
            if (MenuPanel.Visibility == Visibility.Collapsed)
            {
                MenuPanel.Visibility = Visibility.Visible;
            }
            else
            {
                MenuPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void btnStationskarte_Click(object sender, RoutedEventArgs e)
        {
            var mapView = new MapView();
            MainContent.Content = mapView;
            MenuPanel.Visibility = Visibility.Collapsed;
        }


        private void btnUebersicht_Click(object sender, RoutedEventArgs e)
        {
            // Zur Übersichtsseite wechseln
            MainContent.Content = new UebersichtView();
            MenuPanel.Visibility = Visibility.Collapsed;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            // Menü schließen
            MenuPanel.Visibility = Visibility.Collapsed;
        }

        private void btnAenderungen_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new AenderungenView();
            MenuPanel.Visibility = Visibility.Collapsed;
        }


    }
}
