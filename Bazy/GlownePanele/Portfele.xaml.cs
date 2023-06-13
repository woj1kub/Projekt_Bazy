using Microsoft.VisualBasic;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Bazy
{
    /// <summary>
    /// Logika interakcji dla klasy Portfele.xaml
    /// </summary>
    public partial class Portfele : UserControl
    {
        private readonly string ActiveUser;
        //tymczasowa struktura portfelu póki nie zostanie zrobiona klasa dla całego porjektu
        struct Portfele_info
        {
            public Int64? PortfeleId;
            public string nazwa;
            public decimal wartosc;

            public override string ToString()
            {
                return nazwa +" " +wartosc.ToString();
            }
        }
        ObservableCollection<Portfele_info> portfele_dane=new();
        public Portfele(string ActiveUser)
        {
            InitializeComponent();
            this.ActiveUser = ActiveUser;
            ListyPortfeli();
        }
        Portfele_info Info { get; set; }


        private void lbiPortfele_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(portfele_dane.Count > 0) 
                Info = portfele_dane[0];
            
        }
        private void ListyPortfeli()
        {
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT \"Id_Portfelu\", \"Nazwa_Portfelu\", SUM(\"Kwota\") " +
            "FROM \"Portfele\" " +
            "INNER JOIN \"Portfel Gotówkowy\" ON \"Id_Porfelu\" = \"Id_Portfelu\" " +
            "WHERE \"Użykownik\" = @login " +
             "GROUP BY \"Id_Portfelu\", \"Nazwa_Portfelu\"");
            cmd.Parameters.AddWithValue("@login", ActiveUser);

            cmd.Connection=conn;
            NpgsqlDataReader reader= cmd.ExecuteReader();
            Portfele_info portfel;

            while (reader.Read())
            {
                portfel.PortfeleId = reader.GetInt64(0);
                portfel.nazwa = reader.GetString(1);
                portfel.wartosc = reader.GetDecimal(2);
                portfele_dane.Add(portfel);
            }
            conn.Close();
            lbiPortfele.ItemsSource = portfele_dane;
        }
       
        private void btDodaj_Click(object sender, RoutedEventArgs e)
        {
            if(NewPortfelName.Text==string.Empty) 
                return;
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
            if (id_portfelea == null)
                return;

            //uzupełnienie danych w tablicy portfeli
            Portfele_info portfel;
            portfel.PortfeleId =(Int64) id_portfelea;
            portfel.nazwa = NewPortfelName.Text;
            portfel.wartosc =0;
            portfele_dane.Add(portfel);
            
            //Czyszczenie
            Fundusze.Clear();
            NewPortfelName.Clear();
            
        }
        //Do dodania procedura do usuwanie rzeczy
        private void btUsuń_Click(object sender, RoutedEventArgs e)
        {
            if(Info.PortfeleId==null) return;
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd;
            
            cmd = new("DELETE FROM \"Portfel Gotówkowy\" WHERE \"Id_Porfelu\"= @Id_portfel");
            cmd.Parameters.AddWithValue("Id_portfel", Info.PortfeleId);
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();
            //Stworzyć trigger do usuwania rzeczy z bazy danych 
            //if (id_portfelea != null)
            //{
            //    cmd = new("DELETE FROM \"Historia Transakcji Portfelu\" " +
            //        "WHERE \"Id_Portfela_Gotówkowego\"= " +
            //        "(SELECT \"Id_Porfelu\" FROM \"Portfel Gotówkowy\" WHERE \"Id_Porfelu\" = @Id_portfel ) ");
            //    cmd.Parameters.AddWithValue("Id_portfel", id_portfelea);
            //    cmd.Connection = conn;
            //    cmd.ExecuteNonQuery();
            //}

            //cmd = new("DELETE FROM \"Portfele\" WHERE \"Id_Portfelu\"= @Id_portfel ");
            //cmd.Parameters.AddWithValue("Id_portfel", Info.PortfeleId);
            //cmd.Connection = conn;
            //cmd.ExecuteNonQuery();

            portfele_dane.RemoveAt(portfele_dane.IndexOf(Info));

            conn.Close();
        }

        private ObservableCollection<Portfele_info> GetPortfele_dane()
        {
            return portfele_dane;
        }

        private void btDodajFundusze_Click(object sender, RoutedEventArgs e)
        {
            if (Info.PortfeleId==null) return;
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd;
            //Dodanie nowego portfelu gotówkowego
            cmd = new NpgsqlCommand("INSERT INTO \"Portfel Gotówkowy\" (\"Id_Porfelu\" , \"Kwota\") " +
                "VALUES ((SELECT \"Id_Portfelu\" FROM \"Portfele\" WHERE \"Id_Portfelu\" = @Id_portfel ) , @kwotaPortfela)" +
                "RETURNING \"Id_Portfela_Gotówkowego\" ");
            cmd.Parameters.AddWithValue("@kwotaPortfela", decimal.Parse(Fundusze.Text));
            cmd.Parameters.AddWithValue("Id_portfel", Info.PortfeleId);
            cmd.Connection = conn;
            var id_portfel_gotowkowy = cmd.ExecuteScalar();
            
            if (id_portfel_gotowkowy != null)
            {
                //Dodanie w histroii transakcji informacje o stworzeniu nowej histrorii
                cmd = new NpgsqlCommand("INSERT INTO \"Historia Transakcji Portfelu\" " +
                    "(\"Id_Portfela_Gotówkowego\" , \"Kwota\" , \"Data_Transakcji\", \"Opis_Transakcji\") " +
                    "VALUES ((SELECT \"Id_Portfela_Gotówkowego\" FROM \"Portfel Gotówkowy\" WHERE \"Id_Portfela_Gotówkowego\" = @Id_portfel ) , @kwotaPortfela, @data,@opis)");
                cmd.Parameters.AddWithValue("@kwotaPortfela", decimal.Parse(Fundusze.Text));
                cmd.Parameters.AddWithValue("Id_portfel", id_portfel_gotowkowy);
                cmd.Parameters.AddWithValue("@data", DateTime.Now);
                cmd.Parameters.AddWithValue("@opis", "Utworzenie portfela gotówkowego");
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }

            var selectedPortfel = portfele_dane[lbiPortfele.SelectedIndex];
            selectedPortfel.wartosc += decimal.Parse(Fundusze.Text);
            portfele_dane[lbiPortfele.SelectedIndex] = selectedPortfel;

            Fundusze.Clear();
            conn.Close();
        }
        private void btSort_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
