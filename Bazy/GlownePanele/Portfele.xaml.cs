﻿using Microsoft.VisualBasic;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bazy
{
    /// <summary>
    /// Logika interakcji dla klasy Portfele.xaml
    /// </summary>
    public partial class Portfele : UserControl
    {
        private readonly string ActiveUser;
        ObservableCollection<Portfel> portfele_dane = new();
        public event Action<Portfel> ActivePortfel;
        Portfel portfel_wew;

        public Portfele(string ActiveUser, Action<Portfel> ActivePortfel)
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
        }
        private void ListyPortfeli()
        {
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd = new("SELECT \"Id_Portfelu\", \"Nazwa_Portfelu\", SUM(\"Kwota\") " +
            "FROM \"Portfele\" " +
            "INNER JOIN \"Portfel Gotówkowy\" ON \"Id_Porfelu\" = \"Id_Portfelu\" " +
            "WHERE \"Użykownik\" = @login " +
             "GROUP BY \"Id_Portfelu\", \"Nazwa_Portfelu\"");
            cmd.Parameters.AddWithValue("@login", ActiveUser);

            cmd.Connection = conn;
            NpgsqlDataReader reader = cmd.ExecuteReader();
            Portfel portfel;

            while (reader.Read())
            {
                portfel = new Portfel
                {
                    PortfeleId = reader.GetInt64(0),
                    Nazwa = reader.GetString(1),
                    Wartosc = reader.GetDecimal(2)
                };
                portfele_dane.Add(portfel);
            }

            conn.Close();
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

        }
        //Do dodania procedura do usuwanie rzeczy
        private void btUsuń_Click(object sender, RoutedEventArgs e)
        {
            //return;
            if (portfel_wew == null) return;
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd;

            cmd = new("DELETE FROM \"Portfel Gotówkowy\" WHERE \"Id_Porfelu\"= @Id_portfel");
            cmd.Parameters.AddWithValue("Id_portfel", portfel_wew.PortfeleId);
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();
            conn.Close();
            portfele_dane.RemoveAt(portfele_dane.IndexOf(portfel_wew));
            ActivePortfel.Invoke(new Portfel());
        }
        private void btDodajFundusze_Click(object sender, RoutedEventArgs e)
        {
            if (portfel_wew.PortfeleId == null) return;
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd;
            //Dodanie nowego portfelu gotówkowego
            cmd = new NpgsqlCommand("INSERT INTO \"Portfel Gotówkowy\" (\"Id_Porfelu\" , \"Kwota\") " +
                "VALUES ((SELECT \"Id_Portfelu\" FROM \"Portfele\" WHERE \"Id_Portfelu\" = @Id_portfel ) , @kwotaPortfela)" +
                "RETURNING \"Id_Portfela_Gotówkowego\" ");
            cmd.Parameters.AddWithValue("@kwotaPortfela", decimal.Parse(Fundusze.Text));
            cmd.Parameters.AddWithValue("Id_portfel", portfel_wew.PortfeleId);
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
            var selectedIndex = portfele_dane.IndexOf(portfel_wew);
            var selectedPortfel = portfele_dane[selectedIndex];
            selectedPortfel.Wartosc += decimal.Parse(Fundusze.Text);
            lbiPortfele.ItemsSource = new List<Portfel>();
            lbiPortfele.ItemsSource = portfele_dane;
            Fundusze.Clear();
            conn.Close();
        }
        private void btSort_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Fundusze_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Sprawdzenie, czy wprowadzony znak jest cyfrą lub przecinkiem
            if (!char.IsDigit(e.Text, e.Text.Length - 1) && e.Text != ",")
            {
                e.Handled = true; // Zatrzymaj zdarzenie, aby znak nie został wprowadzony do TextBox
            }

            // Sprawdzenie, czy tekst w TextBox ma poprawny format
            string newText = Fundusze.Text + e.Text;
            Regex regex = new Regex(@"^\d+(,\d{0,2})?$");
            if (!regex.IsMatch(newText))
            {
                e.Handled = true; // Zatrzymaj zdarzenie, aby znak nie został wprowadzony do TextBox
            }
        }

    }
}
