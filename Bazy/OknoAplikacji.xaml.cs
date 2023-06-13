
using Npgsql;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bazy
{
    /// <summary>
    /// Logika interakcji dla klasy OknoAplikacji.xaml
    /// </summary>
    public partial class OknoAplikacji : Window
    {
        private readonly string ActiveUser;
        public OknoAplikacji(string ActiveUser)
        {
            this.ActiveUser = ActiveUser;
            InitializeComponent();
            lbUser.Content = "Witaj, "+this.ActiveUser+"!";
            
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btInwestycje_Click(object sender, RoutedEventArgs e)
        {
            Inwestycje inwestycje = new Inwestycje();
            contentControl.Content = inwestycje;
        }
        private void btWyloguj_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            this.Close();
            main.Show();
        }

        private void btPortfele_Click(object sender, RoutedEventArgs e)
        {
            Portfele portfele = new Portfele(ActiveUser);
            contentControl.Content=portfele;
        }

        private void btUstawienia_Click(object sender, RoutedEventArgs e)
        {
            Ustawienia ustawienia = new Ustawienia();
            contentControl.Content = ustawienia;
        }

        private void btHistoria_Click(object sender, RoutedEventArgs e)
        {
            Historie historie = new Historie();
            contentControl.Content = historie;
        }
    }
}
