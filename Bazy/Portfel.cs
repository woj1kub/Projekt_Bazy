using Npgsql;
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
    
    public class Portfel : INotifyPropertyChanged
    {
        private long portfeleId=new();
        private string? nazwa ;
        private decimal wartosc = new();
        public ObservableCollection<Lokaty>? Lokaties { get; set; } = new();
        public ObservableCollection<Obligacje>? Obligacjes { get; set; } = new();
        public ObservableCollection<KontoOszczędnościowe>? KontoOszczędnościowes { get; set; } = new();
        public ObservableCollection<Akcje>? Akcjes { get; set; } = new();
        public ObservableCollection<PortfelGotówkowy>? portfeleGotówkowe = new();

        public Portfel(Portfel portfel)
        {
            this.PortfeleId = portfel.portfeleId;
            this.Nazwa = portfel.nazwa;
            this.Wartosc= portfel.wartosc;
            this.Lokaties= portfel.Lokaties;
            this.Obligacjes = portfel.Obligacjes;
            this.KontoOszczędnościowes = portfel.KontoOszczędnościowes;
            this.Akcjes = portfel.Akcjes;
            this.PortfeleGotówkowe = portfel.portfeleGotówkowe;
        }
        public Portfel(string ActiveUser , string nazwa) : this()
        {
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd;
            //Dodanie nowego portfelu i pobranie jego id
            cmd = new("INSERT INTO \"Portfele\" (\"Użykownik\", \"Nazwa_Portfelu\") " +
                "VALUES ( (SELECT \"Login\" FROM \"Użytkownicy\" WHERE \"Login\" = @user) , @name )" +
                "RETURNING \"Id_Portfelu\"");
            cmd.Parameters.AddWithValue("@user", ActiveUser);
            cmd.Parameters.AddWithValue("@name", nazwa);
            cmd.Connection = conn;
            var id_portfelea = cmd.ExecuteScalar();

            if (id_portfelea == null)
                return;

            PortfeleId = (long)id_portfelea;
            Wartosc = 0;
            Nazwa = nazwa;
            conn.Close();
        }
        public Portfel() { }
        public override string ToString()
        {
            return Nazwa;
        }
        public ObservableCollection<PortfelGotówkowy>? PortfeleGotówkowe
        {
            get { return portfeleGotówkowe; }
            set 
            {
                portfeleGotówkowe= value;
                wartosc = portfeleGotówkowe?.Sum(portfel => portfel.Wartosc) ?? 0;
                OnPropertyChanged(nameof(portfeleGotówkowe));
            }
        }
        public long PortfeleId
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
            if (this.PortfeleId == new long())
                return;

            using (var conn = new NpgsqlConnection(Registration.ConnString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new($"SELECT * FROM \"Lokaty\" WHERE \"Id_Portefelu\" = @idportfel");
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
                        Kwota = reader.GetDecimal(3), 
                        Podatek = reader.GetDouble(4),
                        Data_zakupu = reader.GetDateTime(5),
                        Nazwa=reader.GetString(6),
                        Data_Zakończenia=reader.GetDateTime(7),
                        Kapitalizacjaodesetek = (kapitalizacjaodesetek)reader.GetValue(8)
                    };
                    Lokaties.Add(lokaty);
                }
                conn.Close();
            }

        }
        void DataObligacje()
        {
            if (this.PortfeleId == new long())
                return;

            using (var conn = new NpgsqlConnection(Registration.ConnString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new($"SELECT * FROM \"Obligacje z Stałym Oprocentowaniem\" WHERE \"Id_Portefelu\" = @idportfel");
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
            if (this.PortfeleId == new long())
                return;

            using (var conn = new NpgsqlConnection(Registration.ConnString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new($"SELECT * FROM \"Konto oszczędnościowe\" WHERE \"Id_Portfelu\" = @idportfel");
                cmd.Parameters.AddWithValue("@idportfel", this.PortfeleId);
                cmd.Connection = conn;
                KontoOszczędnościowes = new();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                KontoOszczędnościowe kontoOszczędnościowe = new();
                while (reader.Read())
                {
                    kontoOszczędnościowe = new()
                    {
                        Id_KontaOszczędnościowego = reader.GetInt64(0),
                        Data_Założenia = reader.GetDateTime(2),
                        Kwota = reader.GetDecimal(3),
                        Oprecentowanie = reader.GetDouble(4),
                        Data_Wypłaty_Odsetek = reader.GetDateTime(5),
                        Podatek = reader.GetDouble(6),
                        Nazwa = reader.GetString(7),
                        Kapitalizacja=reader.GetDateTime(8)
                    };
                    KontoOszczędnościowes.Add(kontoOszczędnościowe);
                }
                conn.Close();
            }

        }
        void DataAkcje()
        {
            if (this.PortfeleId == new long())
                return;

            using (var conn = new NpgsqlConnection(Registration.ConnString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new($"SELECT * FROM \"Akcje\" WHERE \"Id_Portfelu\" = @idportfel");
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
            if (this.PortfeleId == new long())
                return;

            using (var conn = new NpgsqlConnection(Registration.ConnString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new($"SELECT * FROM \"Portfel Gotówkowy\" WHERE \"Id_Porfelu\" = @idportfel");
                cmd.Parameters.AddWithValue("@idportfel", this.portfeleId);
                cmd.Connection = conn;
                portfeleGotówkowe = new();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                PortfelGotówkowy portfelGotówkowy;
                while (reader.Read())
                {
                    portfelGotówkowy = new();
                    portfelGotówkowy.IdPortfelGotowkowy= reader.GetInt64(0);
                    portfelGotówkowy.Wartosc = reader.GetDecimal(2);
                        
                    portfeleGotówkowe.Add(portfelGotówkowy);
                }
                conn.Close();
            }
            wartosc = portfeleGotówkowe?.Sum(portfel => portfel.Wartosc) ?? 0;

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
