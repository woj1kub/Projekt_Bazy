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
            if (portfel_wew == null || !portfele_dane.Contains(portfel_wew)) return;
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
            lbiPortfele.SelectedIndex = 0;
        }

        private void btDodajPortfelGotowkowy_Click(object sender, RoutedEventArgs e)
        {
            if (portfel_wew.PortfeleId == null || PortfelGotowkowy.Text==string.Empty || !portfele_dane.Contains(portfel_wew)) return;
            using var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd;
            //Dodanie nowego portfelu gotówkowego
            cmd = new NpgsqlCommand("INSERT INTO \"Portfel Gotówkowy\" (\"Id_Porfelu\" , \"Kwota\") " +
                "VALUES ((SELECT \"Id_Portfelu\" FROM \"Portfele\" WHERE \"Id_Portfelu\" = @Id_portfel ) , @kwotaPortfela)" +
                "RETURNING \"Id_Portfela_Gotówkowego\" ");
            cmd.Parameters.AddWithValue("@kwotaPortfela", decimal.Parse(PortfelGotowkowy.Text));
            cmd.Parameters.AddWithValue("Id_portfel", portfel_wew.PortfeleId);
            cmd.Connection = conn;
            var id_portfel_gotowkowy = cmd.ExecuteScalar();

            if (id_portfel_gotowkowy != null)
            {
                //Dodanie w histroii transakcji informacje o stworzeniu nowej histrorii portfela
                cmd = new NpgsqlCommand("INSERT INTO \"Historia Transakcji Portfelu\" " +
                    "(\"Id_Portfela_Gotówkowego\" , \"Kwota\" , \"Data_Transakcji\", \"Opis_Transakcji\") " +
                    "VALUES ((SELECT \"Id_Portfela_Gotówkowego\" FROM \"Portfel Gotówkowy\" WHERE \"Id_Portfela_Gotówkowego\" = @Id_portfel ) , @kwotaPortfela, @data,@opis)");
                cmd.Parameters.AddWithValue("@kwotaPortfela", decimal.Parse(PortfelGotowkowy.Text));
                cmd.Parameters.AddWithValue("Id_portfel", id_portfel_gotowkowy);
                cmd.Parameters.AddWithValue("@data", DateTime.Now);
                cmd.Parameters.AddWithValue("@opis", "Utworzenie portfela gotówkowego");
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
            var selectedIndex = portfele_dane.IndexOf(portfel_wew);
            var selectedPortfel = portfele_dane[selectedIndex];
            selectedPortfel.Wartosc += decimal.Parse(PortfelGotowkowy.Text);
            portfele_dane[selectedIndex] =new(selectedPortfel);
            lbiPortfele.SelectedIndex = selectedIndex;

            PortfelGotowkowy.Clear();
            conn.Close();
            portfele_dane = new ObservableCollection<Portfel>(portfele_dane.OrderByDescending(item => item.Wartosc));
            lbiPortfele.ItemsSource = portfele_dane;
        
        }
        private void btDodajFundusze_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Fundusze_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1) && e.Text != ",")
            {
                e.Handled = true; 
            }
            string newText = PortfelGotowkowy.Text + e.Text;
            Regex regex = new(@"^\d+(,\d{0,2})?$");
            if (!regex.IsMatch(newText))
            {
                e.Handled = true;
            }
        }

        private void DeletePG_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
