using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Bazy
{
    public partial class PortfelePanel : UserControl
    {
        private readonly string ActiveUser;
        ObservableCollection<Portfel> portfele_dane = new();
        public event Action<Portfel> ActivePortfel;
        Portfel portfel_wew;

        public PortfelePanel(string ActiveUser, Action<Portfel> ActivePortfel)
        {
            portfele_dane = new();
            portfel_wew = new();
            InitializeComponent();
            this.ActiveUser = ActiveUser;
            this.ActivePortfel += ActivePortfel;
            ListyPortfeli();

        }

        private void lbiPortfele_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int chosen = lbiPortfele.SelectedIndex;
            if (chosen < 0)
            {
                if (ActivePortfel == null)
                {
                    return;
                }
                chosen = portfele_dane.IndexOf(portfel_wew);
                if (chosen < 0)
                {
                    return;
                }
            }

            ActivePortfel.Invoke(portfele_dane[chosen]);
            portfel_wew = portfele_dane[chosen];
            lbiPortfele.SelectedIndex = chosen;
            lbPortfeleGotówkowe.ItemsSource = portfel_wew.portfeleGotówkowe;
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
            lbiPortfele.ItemsSource = portfele_dane;
        }
        
        private void btDodaj_Click(object sender, RoutedEventArgs e)
        {
            if (NewPortfelName.Text == string.Empty)
                return;
            
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd;
            //Dodanie nowego portfelu i pobranie jego id
            cmd = new("INSERT INTO \"Portfele\" (\"Użykownik\", \"Nazwa_Portfelu\") " +
                "VALUES ( (SELECT \"Login\" FROM \"Użytkownicy\" WHERE \"Login\" = @user) , @name )" +
                "RETURNING \"Id_Portfelu\"");
            cmd.Parameters.AddWithValue("@user", ActiveUser);
            cmd.Parameters.AddWithValue("@name", NewPortfelName.Text);
            cmd.Connection = conn;
            var id_portfelea = cmd.ExecuteScalar();

            if (id_portfelea == null)
                return;

            conn.Close();

            //uzupełnienie danych w tablicy portfeli
            Portfel portfel = new()
            {
                PortfeleId = (Int64)id_portfelea,
                Nazwa = NewPortfelName.Text,
                Wartosc = 0
            };
            portfele_dane.Add(portfel);

            //Czyszczenie
            NewPortfelName.Clear();
            portfele_dane = new ObservableCollection<Portfel>(portfele_dane.OrderByDescending(item => item.Wartosc));
            lbiPortfele.ItemsSource = portfele_dane;
        }

        private void btUsuń_Click(object sender, RoutedEventArgs e)
        {
            if (portfel_wew == null || !portfele_dane.Contains(portfel_wew) ) return;
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd;

            cmd = new("DELETE FROM \"Portfele\" WHERE \"Id_Portfelu\"= @Id_portfel");
            cmd.Parameters.AddWithValue("Id_portfel", portfel_wew.PortfeleId);
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();
            conn.Close();
            portfele_dane.RemoveAt(portfele_dane.IndexOf(portfel_wew));
            ActivePortfel.Invoke(new Portfel());
            lbiPortfele.SelectedIndex = -1;
            lbPortfeleGotówkowe.ItemsSource=null;
        }

        private void btDodajPortfelGotowkowy_Click(object sender, RoutedEventArgs e)
        {
            if (portfel_wew == null || PortfelGotowkowy.Text==string.Empty || !portfele_dane.Contains(portfel_wew)) return;

            decimal wartosc = decimal.Parse(PortfelGotowkowy.Text);

            RestartPortfela();
            portfele_dane = new ObservableCollection<Portfel>(portfele_dane.OrderByDescending(item => item.Wartosc));
            lbiPortfele.ItemsSource = portfele_dane;
            portfel_wew.portfeleGotówkowe.Add(new PortfelGotówkowy(wartosc: wartosc , idPorfela:(long) portfel_wew.PortfeleId));
            PortfelGotowkowy.Clear();

        }

        private void _PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1) && e.Text != ",")
            {
                e.Handled = true; 
            }
            string newText = ((TextBox) sender).Text + e.Text;
            Regex regex = new(@"^\d+(,\d{0,2})?$");
            if (!regex.IsMatch(newText))
            {
                e.Handled = true;
            }
        }

        private void DeletePG_Click(object sender, RoutedEventArgs e)
        {
            if (lbPortfeleGotówkowe.SelectedIndex == -1 || portfel_wew.portfeleGotówkowe==null || portfel_wew == null || UsuFundusze.Text == string.Empty || !portfele_dane.Contains(portfel_wew)) return;
            decimal wartosc =decimal.Parse(DodFundusze.Text);
            portfel_wew.portfeleGotówkowe[lbPortfeleGotówkowe.SelectedIndex].ZmianaWartości(-wartosc, "Pobranie z portfela gotówkowego");
            
            var selectedPortfel = portfel_wew.portfeleGotówkowe[lbPortfeleGotówkowe.SelectedIndex];
            portfel_wew.portfeleGotówkowe[lbPortfeleGotówkowe.SelectedIndex] = new(selectedPortfel);
            portfel_wew.Wartosc -= wartosc;
            UsuFundusze.Text = "";

            RestartPortfela();

        }

        private void btDodajFundusze_Click(object sender, RoutedEventArgs e)
        {
            if (lbPortfeleGotówkowe.SelectedIndex==-1 || portfel_wew.portfeleGotówkowe == null || portfel_wew == null || DodFundusze.Text == string.Empty || !portfele_dane.Contains(portfel_wew)) return;
            decimal wartosc = decimal.Parse(DodFundusze.Text);
            portfel_wew.portfeleGotówkowe[lbPortfeleGotówkowe.SelectedIndex].ZmianaWartości(wartosc, "Wpłacenie do portfela gotówkowego");
            
            var selectedPortfel = portfel_wew.portfeleGotówkowe[lbPortfeleGotówkowe.SelectedIndex];
            portfel_wew.portfeleGotówkowe[lbPortfeleGotówkowe.SelectedIndex] = new(selectedPortfel);
            portfel_wew.Wartosc += wartosc;
            DodFundusze.Text = "";

            RestartPortfela();
        }

        void RestartPortfela() 
        {
            var selectedIndex = portfele_dane.IndexOf(portfel_wew);
            var selectedPortfel = portfele_dane[selectedIndex];
            selectedPortfel.Wartosc += decimal.Parse(PortfelGotowkowy.Text);
            portfele_dane[selectedIndex] = new(selectedPortfel);
            lbiPortfele.SelectedIndex = selectedIndex;
        }
        
    }
}
