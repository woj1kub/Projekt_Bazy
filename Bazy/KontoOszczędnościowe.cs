using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override string ToString()
        {
            return $"{Id_KontaOszczędnościowego} {Data_Założenia} {Kwota} {Oprecentowanie} {Data_Wypłaty_Odsetek} {Podatek} {Nazwa}";
        }

        public KontoOszczędnościowe(DateTime data_Założenia, decimal kwota, double oprecentowanie, DateTime? data_Wypłaty_Odsetek, double podatek, string nazwa)
        {
            Data_Założenia = data_Założenia;
            Kwota = kwota;
            Oprecentowanie = oprecentowanie;
            Data_Wypłaty_Odsetek = data_Wypłaty_Odsetek;
            Podatek = podatek;
            Nazwa = nazwa;
        }
        public KontoOszczędnościowe() { }

        public void AddToDatabase(Portfel p)
        {
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd = new("INSERT INTO \"Konto oszczędnościowe\" (\"Id_Portfelu\", \"Data_Założenia\"" +
                ", \"Kwota\", \"Oprocentowanie\", \"Data_Wypłaty_Odsetek\", \"Podatek\",\"Nazwa\") " +
                "VALUES (@Idportfelu, @DataAktualna, @Kwota, @Oprocentowanie, @DataWypłaty, @Podatek, @Nazwa)"
                + "RETURNING \"Id_Konta_Oszczędnościowego\"");
            cmd.Parameters.AddWithValue("@Idportfelu", p.PortfeleId);
            cmd.Parameters.AddWithValue("@DataAktualna", DateTime.Now);
            cmd.Parameters.AddWithValue("@Kwota", this.Kwota);
            cmd.Parameters.AddWithValue("@Oprocentowanie", this.Oprecentowanie);
            cmd.Parameters.AddWithValue("@DataWypłaty", this.Data_Wypłaty_Odsetek);
            cmd.Parameters.AddWithValue("@Podatek",this.Podatek);
            cmd.Parameters.AddWithValue("@Nazwa", this.Nazwa);
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();
            conn.Close();

        }
    }
}
