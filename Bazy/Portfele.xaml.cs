using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bazy
{
    /// <summary>
    /// Logika interakcji dla klasy Portfele.xaml
    /// </summary>
    public partial class Portfele : UserControl
    {
        private readonly string ActiveUser;
        struct Portfel_Gotówkowy
        {
            public string nazwa;
            public decimal wartosc;

            public override string ToString()
            {
                return nazwa +" " +wartosc.ToString();
            }
        }
        ObservableCollection<Portfel_Gotówkowy> portfele_dane=new();
        public string ActivePortfef { set; get; }=string.Empty;
        public Portfele(string ActiveUser)
        {
            InitializeComponent();
            this.ActiveUser = ActiveUser;
            ListyPortfeli();
        }

        private void lbiPortfele_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActivePortfef = lbiPortfele.SelectedValue.ToString();
        }
        private void ListyPortfeli()
        {
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT \"Nazwa_Portfelu\",\"Kwota\" " +
                "FROM \"Portfele\" INNER JOIN \"Portfel Gotówkowy\" ON \"Id_Porfelu\"=\"Id_Portfelu\" " +
                "WHERE \"Użykownik\"=@login");

            cmd.Parameters.AddWithValue("@login", ActiveUser);
            cmd.Connection=conn;
            NpgsqlDataReader reader= cmd.ExecuteReader();
            Portfel_Gotówkowy portfel;

            while (reader.Read())
            {
                portfel.nazwa = reader.GetString(0);
                portfel.wartosc = reader.GetDecimal(1);
                portfele_dane.Add(portfel);
            }
            conn.Close();
            Wyswietlanie();
        }
        void Wyswietlanie()
        {
            lbiPortfele.ItemsSource = portfele_dane;
            cbPortfel1.ItemsSource = portfele_dane;    
            cbPortfel2.ItemsSource = portfele_dane;    
        }
        private void btDodaj_Click(object sender, RoutedEventArgs e)
        {
            if(NewPortfelName.Text==string.Empty || Fundusze.Text==string.Empty) 
            {
                return;
            }
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd;
            //Dodanie nowego portfelu i pobranie jego id
            cmd = new("INSERT INTO \"Portfele\" (\"Użykownik\", \"Nazwa_Portfelu\") " +
                "VALUES ( (SELECT \"Login\" FROM \"Użytkownicy\" WHERE \"Login\" = @user) , @name )" +
                "RETURNING \"Id_Portfelu\"" );
            cmd.Parameters.AddWithValue("@user", ActiveUser);
            cmd.Parameters.AddWithValue("@name", NewPortfelName.Text);
            cmd.Connection=conn;
            var id_portfelea = cmd.ExecuteScalar();
            //Dodanie nowego portfelu gotówkowego
            cmd = new NpgsqlCommand("INSERT INTO \"Portfel Gotówkowy\" (\"Id_Porfelu\" , \"Kwota\") " +
                "VALUES ((SELECT \"Id_Portfelu\" FROM \"Portfele\" WHERE \"Id_Portfelu\" = @Id_portfel ) , @kwotaPortfela)");
            cmd.Parameters.AddWithValue("@kwotaPortfela", decimal.Parse(Fundusze.Text) );
            cmd.Parameters.AddWithValue("Id_portfel", id_portfelea);

            cmd.Connection = conn;
            cmd.ExecuteNonQuery();
            conn.Close();
            Portfel_Gotówkowy portfel;
            portfel.nazwa = NewPortfelName.Text;
            portfel.wartosc =decimal.Parse( Fundusze.Text);
            portfele_dane.Add(portfel);
            
            Fundusze.Clear();
            NewPortfelName.Clear();
        }

    }
}
