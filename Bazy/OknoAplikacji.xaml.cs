
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
        private Portfel ActivePortfel { get; set; }

        public OknoAplikacji(string ActiveUser)
        {  
            
            this.ActiveUser = ActiveUser;
            InitializeComponent();
            lbUser.Content = "Witaj, "+this.ActiveUser+"!";
            ActivePortfel = new Portfel
            {
                Wartosc = 0,
                Nazwa = "",
                PortfeleId = 0
            };

            DataContext = this;
            Portfele portfele = new(ActiveUser, ChildWindow_VariableChanged);
            contentControl.Content = portfele;
            lbFundusze.Visibility = Visibility.Visible;


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
            Portfele portfele = new Portfele(ActiveUser,ChildWindow_VariableChanged);
            contentControl.Content=portfele;
            
        }
        private void ChildWindow_VariableChanged(Portfel newValue)
        {
            ActivePortfel = newValue;
            lbFundusze.Content = ActivePortfel.Wartosc;
            lbFundusze.DataContext = ActivePortfel;
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
