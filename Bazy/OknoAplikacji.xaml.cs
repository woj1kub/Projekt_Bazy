
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
            var conn = new NpgsqlConnection(Registration.ConnString());
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

    }
}
