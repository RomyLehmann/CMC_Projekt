using CMC_Projekt;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CMC_Projekt.View
{
    public partial class AenderungenView : UserControl
    {
        private string targetBettNummer;

        public AenderungenView() : this(null)
        {
        }

        public AenderungenView(string bettNummer)
        {
            InitializeComponent();
            targetBettNummer = bettNummer;
            LadeBettenDaten();
        }

        private void LadeBettenDaten()
        {
            var bettenListe = BedDataManager.GetAlleBetten();
            BettControl targetControl = null;

            foreach (var bett in bettenListe)
            {
                var control = new BettControl(bett);
                BettenContainer.Children.Add(control);

                if (!string.IsNullOrEmpty(targetBettNummer) && bett.BettNummer == targetBettNummer)
                {
                    targetControl = control;
                }
            }

            if (targetControl != null)
            {
                this.Loaded += (s, e) => ScrollAndHighlightBett(targetControl);
            }
        }

        private void ScrollAndHighlightBett(BettControl control)
        {
            control.BringIntoView();
            control.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 193, 7));
            control.BorderThickness = new Thickness(4);

            var animation = new ColorAnimation
            {
                From = Color.FromRgb(255, 193, 7),
                To = Colors.White,
                Duration = new Duration(System.TimeSpan.FromSeconds(0.5)),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(3)
            };

            var brush = new SolidColorBrush();
            control.Background = brush;
            brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }
    }

    public class BettControl : Border
    {
        private BettData bettData;
        private ComboBox statusComboBox;
        private ComboBox wartungComboBox;

        public BettControl(BettData bett)
        {
            this.bettData = bett;
            BorderBrush = Brushes.Black;
            BorderThickness = new Thickness(1);
            Margin = new Thickness(0, 0, 0, 15);
            Padding = new Thickness(20);
            Background = Brushes.White;

            var mainGrid = new Grid();
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });

            var bettLabel = new TextBlock
            {
                Text = $"Bett: {bett.BettNummer}\nZimmer: {bett.Zimmer}",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 20, 0)
            };
            Grid.SetColumn(bettLabel, 0);
            mainGrid.Children.Add(bettLabel);

            var statusStack = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 15, 0) };
            Grid.SetColumn(statusStack, 1);

            var statusLabel = new TextBlock
            {
                Text = "Status:",
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            };
            statusStack.Children.Add(statusLabel);

            statusComboBox = new ComboBox
            {
                FontSize = 14,
                Padding = new Thickness(10, 8, 10, 8),
                Height = 35,
                Cursor = System.Windows.Input.Cursors.Hand
            };
            statusComboBox.Items.Add("Frei");
            statusComboBox.Items.Add("Belegt");
            statusComboBox.SelectedItem = bett.Status;
            statusStack.Children.Add(statusComboBox);

            mainGrid.Children.Add(statusStack);

            var wartungStack = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 15, 0) };
            Grid.SetColumn(wartungStack, 2);

            var wartungLabel = new TextBlock
            {
                Text = "Wartung:",
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            };
            wartungStack.Children.Add(wartungLabel);

            wartungComboBox = new ComboBox
            {
                FontSize = 14,
                Padding = new Thickness(10, 8, 10, 8),
                Height = 35,
                Cursor = System.Windows.Input.Cursors.Hand
            };
            wartungComboBox.Items.Add("Sauber");
            wartungComboBox.Items.Add("Nicht sauber");
            wartungComboBox.SelectedItem = bett.Wartung;
            wartungStack.Children.Add(wartungComboBox);

            mainGrid.Children.Add(wartungStack);

            var speichernButton = new Button
            {
                Content = "Speichern",
                Width = 130,
                Height = 40,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromRgb(39, 174, 96)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                VerticalAlignment = VerticalAlignment.Center
            };
            speichernButton.Click += (s, e) => SpeichernClicked();
            Grid.SetColumn(speichernButton, 3);
            mainGrid.Children.Add(speichernButton);

            Child = mainGrid;
        }

        private void SpeichernClicked()
        {
            if (statusComboBox.SelectedItem == null || wartungComboBox.SelectedItem == null)
            {
                MessageBox.Show("Bitte wählen Sie Status und Wartung aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string neuerStatus = statusComboBox.SelectedItem.ToString();
            string neueWartung = wartungComboBox.SelectedItem.ToString();

            // ECHTES SPEICHERN in den zentralen Datenmanager
            bool erfolg = BedDataManager.UpdateBett(bettData.BettNummer, neuerStatus, neueWartung);

            if (erfolg)
            {
                // Lokale Daten auch aktualisieren
                bettData.Status = neuerStatus;
                bettData.Wartung = neueWartung;

                MessageBox.Show(
                    $"Änderungen für {bettData.BettNummer} gespeichert!\n\nStatus: {bettData.Status}\nWartung: {bettData.Wartung}",
                    "Erfolgreich gespeichert",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Fehler beim Speichern!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
