﻿using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Bazy
{
    public class PortfelGotówkowy : INotifyPropertyChanged
    {
        private Int64? idPorfelGotowkowy;
        private decimal wartosc;
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Int64? IdPortfelGotowkowy { 
            get {  return idPorfelGotowkowy; } 
            set
            {
                idPorfelGotowkowy = value;
                OnPropertyChanged(nameof(idPorfelGotowkowy));
            }
        }
        public decimal Wartosc { get {return wartosc ; }
            set
            {
                wartosc = value;
                OnPropertyChanged(nameof(wartosc));
            }        
        }
    }

    public class Portfel : INotifyPropertyChanged
    {
        private Int64? portfeleId;
        private string? nazwa;
        private decimal wartosc;
        public ObservableCollection<Lokaty>? Lokaties { get; set; }
        public ObservableCollection<Obligacje>? Obligacjes { get; set; }
        public ObservableCollection<KontoOszczędnościowe>? KontoOszczędnościowes { get; set; }
        public ObservableCollection<Akcje>? Akcjes { get; set; }
        public ObservableCollection<PortfelGotówkowy>? PortfeleGotówkowe { get; set; }

        public Portfel(Int64? portfelId, string? nazwa, decimal wartość)
        {
            this.Nazwa = nazwa;
            this.Wartosc = wartość;
            this.PortfeleId = portfelId;
        }
        public Portfel() { }

        public Int64? PortfeleId
        {
            get { return portfeleId; }
            set { 
                portfeleId = value; 
                OnPropertyChanged(nameof(PortfeleId));
                }
        }

        public string? Nazwa 
        {
            get { return nazwa; }
            set { nazwa = value; OnPropertyChanged(nameof(Nazwa));}
        }

        public decimal Wartosc 
        {
            get { 
                return wartosc;
            }
            set {
                wartosc = value; OnPropertyChanged(nameof(Wartosc));
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
       
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void DataLokaty()
        {
            if (this.PortfeleId != null)
                return;

            using (var conn = new NpgsqlConnection(Registration.ConnString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new($"SELECT * FROM \"LOKATY\" WHERE \"Id_Portelefu\" = @idportfel");
                cmd.Parameters.AddWithValue("@idportfel", this.PortfeleId);
                Lokaties = new();
                cmd.Connection = conn;
                NpgsqlDataReader reader = cmd.ExecuteReader();
                Lokaty lokaty = new();
                while (reader.Read()) 
                {
                    lokaty = new()
                    {
                        Id_Lokaty = reader.GetInt64(0),
                        Oprocentowanie = reader.GetDouble(2),
                        Czas = reader.GetInt32(3),
                        Skala = reader.GetInt32(4),
                        Kwota = reader.GetDecimal(5),
                        Podatek = reader.GetDouble(6),
                        Data_zakupu = reader.GetDateTime(7).Date,
                        Nazwa=reader.GetString(8)
                    };
                    Lokaties.Add(lokaty);
                }
                conn.Close();
            }

        }
        void DataObligacje()
        {
            if (this.PortfeleId != null)
                return;

            using (var conn = new NpgsqlConnection(Registration.ConnString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new($"SELECT * FROM \"Obligacje z Stałym Oprocentowaniem\" WHERE \"Id_Portelefu\" = @idportfel");
                cmd.Parameters.AddWithValue("@idportfel", this.PortfeleId);
                Obligacjes= new();
                cmd.Connection = conn;
                NpgsqlDataReader reader = cmd.ExecuteReader();
                Obligacje obligacje = new Obligacje();
                while (reader.Read())
                {
                    obligacje = new()
                    {
                        Id_Obligacji = reader.GetInt32(0),
                        Oprecentowanie = reader.GetDouble(2),
                        Długość_Inwestycji = reader.GetInt32(3),
                        Liczba_Jednostek = reader.GetInt32(4),
                        Kwota_Jednostki = reader.GetDecimal(5),
                        Skala = reader.GetInt32(6),
                        Data_zakupu = reader.GetDateTime(7),
                        Podatek = reader.GetDouble(8),
                        Nazwa = reader.GetString(9)
                    };
                    Obligacjes.Add(obligacje);
                }
                conn.Close();
            }

        }
        void DataKontoOszczednosciowe()
        {
            if (this.PortfeleId != null)
                return;

            using (var conn = new NpgsqlConnection(Registration.ConnString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new($"SELECT * FROM \"Konto oszczędnościowe\" WHERE \"Id_Portelefu\" = @idportfel");
                cmd.Parameters.AddWithValue("@idportfel", this.PortfeleId);
                cmd.Connection = conn;
                KontoOszczędnościowes = new();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                KontoOszczędnościowe kontoOszczędnościowe = new();
                while (reader.Read())
                {
                    kontoOszczędnościowe = new()
                    {
                        Id_KontaOszczędnościowego = reader.GetInt32(0),
                        Data_Założenia = reader.GetDateTime(2),
                        Kwota = reader.GetDecimal(3),
                        Oprecentowanie = reader.GetDouble(4),
                        Data_Wypłaty_Odsetek = reader.GetDateTime(5),
                        Podatek = reader.GetDouble(6),
                        Nazwa = reader.GetString(7),
                    };
                    KontoOszczędnościowes.Add(kontoOszczędnościowe);
                }
                conn.Close();
            }

        }
        void DataAkcje()
        {
            if (this.PortfeleId != null)
                return;

            using (var conn = new NpgsqlConnection(Registration.ConnString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new($"SELECT * FROM \"Akcje\" WHERE \"Id_Portelefu\" = @idportfel");
                cmd.Parameters.AddWithValue("@idportfel", this.PortfeleId);
                cmd.Connection = conn;
                Akcjes = new();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                Akcje akcje= new();
                while (reader.Read())
                {
                    akcje = new()
                    {
                        Id_akcji=reader.GetInt32(0),
                        Walor=reader.GetString(2),
                        Data_Zakupu=reader.GetDateTime(3),
                        Liczba_Jednestek=reader.GetInt32(4),
                        Nazwa=reader.GetString(5),
                    };
                    Akcjes.Add(akcje);
                }
                conn.Close();
            }
        }
        void DataPortfelGotówkowy()
        {
            if (this.PortfeleId != null)
                return;

            using (var conn = new NpgsqlConnection(Registration.ConnString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new($"SELECT * FROM \"Akcje\" WHERE \"Id_Portelefu\" = @idportfel");
                cmd.Parameters.AddWithValue("@idportfel", this.portfeleId);
                cmd.Connection = conn;
                PortfeleGotówkowe = new();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                PortfelGotówkowy portfelGotówkowy= new();
                while (reader.Read())
                {
                    portfelGotówkowy = new()
                    {
                        IdPortfelGotowkowy=reader.GetInt32(0),
                        Wartosc=reader.GetDecimal(2)
                    };
                    PortfeleGotówkowe.Add(portfelGotówkowy);
                }
                conn.Close();
            }

        }

        public void DanePortfela()
        {
            DataPortfelGotówkowy();
            DataObligacje();
            DataAkcje();
            DataKontoOszczednosciowe();
            DataLokaty();
        }
    }

}
