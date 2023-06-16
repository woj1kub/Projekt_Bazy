
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
            PortfelePanel portfele = new(ActiveUser, ChildWindow_VariableChanged);
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
            InwestycjePanel inwestycje = new();
            contentControl.Content = inwestycje;
        }
        private void btWyloguj_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new();
            this.Close();
            main.Show();
        }

        private void btPortfele_Click(object sender, RoutedEventArgs e)
        {
            PortfelePanel portfele = new(ActiveUser,ChildWindow_VariableChanged);
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
            UstawieniaPanel ustawienia = new();
            contentControl.Content = ustawienia;
        }

        private void btHistoria_Click(object sender, RoutedEventArgs e)
        {
            HistoriePanel historie = new();
            contentControl.Content = historie;
        }
    }
}
