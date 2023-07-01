
using Bazy.GlownePanele;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Bazy
{
    /// <summary>
    /// Logika interakcji dla klasy OknoAplikacji.xaml
    /// </summary>


    public partial class OknoAplikacji : Window
    {
        private readonly string ActiveUser;
        ObservableCollection<Portfel> portfele_dane=new();
        
        public OknoAplikacji(string ActiveUser)
        {
            this.ActiveUser = ActiveUser;
            InitializeComponent();
            KontoOszczędnościowe.KapitalizacjaOdsetekKontaOszczędnościowe();
            lbUser.Content = "Witaj, "+this.ActiveUser+"!";
            DataContext = this;
            ListyPortfeli();
            PortfelePanel portfele = new(ActiveUser, portfele_dane, ChildWindow_VariableChanged);        
            contentControl.Content = portfele;
        }
        private void ListyPortfeli()
        {
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd = new("SELECT \"Id_Portfelu\", \"Nazwa_Portfelu\" " +
            "FROM \"Portfele\" " +
            "LEFT JOIN \"Portfel Gotówkowy\" ON \"Id_Porfelu\" = \"Id_Portfelu\" " +
            "WHERE \"Użykownik\" = @login " +
             "GROUP BY \"Id_Portfelu\", \"Nazwa_Portfelu\"");
            cmd.Parameters.AddWithValue("@login", ActiveUser);

            cmd.Connection = conn;
            NpgsqlDataReader reader = cmd.ExecuteReader();
            Portfel portfel;

            while (reader.Read())
            {
                portfel = new()
                {
                    Nazwa = reader.GetString(1),
                    PortfeleId = reader.GetInt64(0)
                };
                portfel.DanePortfela();

                portfele_dane.Add(portfel);
            }

            conn.Close();
            portfele_dane = new ObservableCollection<Portfel>(portfele_dane.OrderByDescending(item => item.Wartosc));
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
            InwestycjePanel inwestycje = new(portfele_dane, ChildWindow_VariableChanged);
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
            PortfelePanel portfele = new(ActiveUser, portfele_dane, ChildWindow_VariableChanged);
            contentControl.Content=portfele;
            
        }
        private void ChildWindow_VariableChanged(ObservableCollection<Portfel> newValue)
        {
            portfele_dane = newValue;
        }

        private void btUstawienia_Click(object sender, RoutedEventArgs e)
        {
            UstawieniaPanel ustawienia = new(ActiveUser);
            contentControl.Content = ustawienia;
        }

        private void btHistoria_Click(object sender, RoutedEventArgs e)
        {
            HistoriePanel historie = new();
            historie.UzupelnijHistorie();
            contentControl.Content = historie;
        }

        private void btRaport_Click(object sender, RoutedEventArgs e)
        {
            RaportPanel raport = new();
            contentControl.Content = raport;
        }


    }
}
