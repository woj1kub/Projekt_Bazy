using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bazy
{
    public class KontoOszczędnościowe
    {
        public long Id_KontaOszczędnościowego { get; set; }
        public DateTime Data_Założenia { get; set; }
        public decimal Kwota { get; set; }
        public double Oprecentowanie { get; set; }
        public DateTime? Data_Wypłaty_Odsetek { get; set; }
        public double Podatek { get; set; }
        public string Nazwa { get; set; }
        public DateTime? Kapitalizacja { get; set; }

        public override string ToString()
        {
            return $"{Id_KontaOszczędnościowego} {Data_Założenia} {Kwota} {Oprecentowanie} {Data_Wypłaty_Odsetek} {Podatek} {Nazwa} {Kapitalizacja}";
        }

        public KontoOszczędnościowe(DateTime data_Założenia, decimal kwota, double oprecentowanie, DateTime? data_Wypłaty_Odsetek
            ,double podatek, string nazwa, DateTime? kapitalizacja)
        {
            Data_Założenia = data_Założenia;
            Kwota = kwota;
            Oprecentowanie = oprecentowanie;
            Data_Wypłaty_Odsetek = data_Wypłaty_Odsetek;
            Podatek = podatek;
            Nazwa = nazwa;
            Kapitalizacja= kapitalizacja;
        }
        public KontoOszczędnościowe() { }

        public void AddToDatabase(Portfel p)
        {
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd = new("INSERT INTO \"Konto oszczędnościowe\" (\"Id_Portfelu\", \"Data_Założenia\"" +
                ", \"Kwota\", \"Oprocentowanie\", \"Data_Wypłaty_Odsetek\", \"Podatek\",\"Nazwa\",\"Kapitalizacja\") " +
                "VALUES (@Idportfelu, @DataAktualna, @Kwota, @Oprocentowanie, @DataWypłaty, @Podatek, @Nazwa, @Kapitalizacja)"
                + "RETURNING \"Id_Konta_Oszczędnościowego\"");
            cmd.Parameters.AddWithValue("@Idportfelu", p.PortfeleId);
            cmd.Parameters.AddWithValue("@DataAktualna", DateTime.Now);
            cmd.Parameters.AddWithValue("@Kwota", this.Kwota);
            cmd.Parameters.AddWithValue("@Oprocentowanie", this.Oprecentowanie);
            cmd.Parameters.AddWithValue("@DataWypłaty", this.Data_Wypłaty_Odsetek);
            cmd.Parameters.AddWithValue("@Podatek",this.Podatek);
            cmd.Parameters.AddWithValue("@Nazwa", this.Nazwa);
            cmd.Parameters.AddWithValue("@Kapitalizacja", this.Kapitalizacja);
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();
            conn.Close();

        }

        public static void KapitalizacjaOdsetekKontaOszczędnościowe()
        {
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd = new($"SELECT * FROM \"Konto oszczędnościowe\"");
            ObservableCollection<KontoOszczędnościowe> kontaKapitalizacja = new();
            cmd.Connection = conn;
            NpgsqlDataReader reader = cmd.ExecuteReader();
            KontoOszczędnościowe konto = new();
            while (reader.Read())
            {
                konto = new()
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
                kontaKapitalizacja.Add(konto);
            }
            conn.Close();

            for (int i = 0; i < kontaKapitalizacja.Count; i++)
            {
                TimeSpan? przedzialCzasu = kontaKapitalizacja[i].Data_Wypłaty_Odsetek - kontaKapitalizacja[i].Data_Założenia;
                DateTime czasAktualny=DateTime.Today;
                DateTime? czasKapitalizacja = kontaKapitalizacja[i].Kapitalizacja;

                if (czasAktualny>=czasKapitalizacja)
                {
                    //algorytm obliczania kwoty
                    decimal kwotaNowa = kontaKapitalizacja[i].Kwota;
                    decimal zyskBezPodatku = kontaKapitalizacja[i].Kwota * (decimal)kontaKapitalizacja[i].Oprecentowanie;
                    decimal podatek = zyskBezPodatku * (decimal)kontaKapitalizacja[i].Podatek;
                    decimal zysk = zyskBezPodatku - podatek;
                    kwotaNowa += zysk;

                    //update pola Kwota oraz update pola Kapitalizacja na następny termin(czasKapitalizacja + przedziałCzasu)
                    DateTime? kapitalizacjaNowa = czasKapitalizacja + przedzialCzasu;
                    var conn2 = new NpgsqlConnection(Registration.ConnString());
                    conn2.Open();
                    NpgsqlCommand cmd2 = new($"UPDATE \"Konto oszczędnościowe\" SET \"Kwota\" = @kwota , \"Kapitalizacja\" = @kapitalizacja"
                        + " WHERE \"Id_Konta_Oszczędnościowego\" = @idkonta");
                    cmd2.Parameters.AddWithValue("@idkonta", kontaKapitalizacja[i].Id_KontaOszczędnościowego);
                    cmd2.Parameters.AddWithValue("@kwota", kwotaNowa);
                    cmd2.Parameters.AddWithValue("@kapitalizacja", kapitalizacjaNowa);
                    cmd2.Connection = conn2;
                    cmd2.ExecuteNonQuery();
                    conn2.Close();
                }
            }
        }
    }
}
