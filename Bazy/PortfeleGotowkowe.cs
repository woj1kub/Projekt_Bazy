using Npgsql;
using System;
using System.ComponentModel;

namespace Bazy
{
    public class PortfelGotówkowy : INotifyPropertyChanged
    {
        private long idPortfelGotowkowy;
        private decimal wartosc;
        public PortfelGotówkowy(PortfelGotówkowy portfelGotówkowy)
        {
            this.IdPortfelGotowkowy = portfelGotówkowy.IdPortfelGotowkowy;
            this.Wartosc = portfelGotówkowy.Wartosc;
        }
        public PortfelGotówkowy() { }
        public PortfelGotówkowy(decimal wartosc, long idPorfela)
        {
            using var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd;
            //decimal wartosc = decimal.Parse(PortfelGotowkowy.Text);
            //Dodanie nowego portfelu gotówkowego
            cmd = new NpgsqlCommand("INSERT INTO \"Portfel Gotówkowy\" (\"Id_Porfelu\" , \"Kwota\") " +
                "VALUES ((SELECT \"Id_Portfelu\" FROM \"Portfele\" WHERE \"Id_Portfelu\" = @Id_portfel ) , @kwotaPortfela)" +
                "RETURNING \"Id_Portfela_Gotówkowego\" ");
            cmd.Parameters.AddWithValue("@kwotaPortfela", wartosc);
            cmd.Parameters.AddWithValue("Id_portfel", idPorfela);
            cmd.Connection = conn;
            IdPortfelGotowkowy = (long) cmd.ExecuteScalar();
            this.Wartosc=wartosc;
            conn.Close();
            TworzenieHistori(wartosc, "Utworzenie portfela gotówkowego");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public long IdPortfelGotowkowy
        {
            get { return idPortfelGotowkowy; }
            set
            {
                idPortfelGotowkowy = value;
                OnPropertyChanged(nameof(idPortfelGotowkowy));
            }
        }
        public decimal Wartosc
        {
            get { return wartosc; }
            set
            {
                wartosc = value;
                OnPropertyChanged(nameof(wartosc));
            }
        }
        public override string ToString()
        {
            return wartosc.ToString();
        }
        //Jeżeli chciecie wprowadzić jakieś informacje w historie portfeli gotówkowych, podajesz wartosc i opis
        //oczywiście dla portfela, którego wywołujesz
        public void TworzenieHistori(decimal wartosc, string opis)
        {
            using var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();

            var cmd = new NpgsqlCommand("INSERT INTO \"Historia Transakcji Portfelu\" " +
                   "(\"Id_Portfela_Gotówkowego\" , \"Kwota\" , \"Data_Transakcji\", \"Opis_Transakcji\") " +
                   "VALUES ((SELECT \"Id_Portfela_Gotówkowego\" FROM \"Portfel Gotówkowy\" WHERE \"Id_Portfela_Gotówkowego\" = @Id_portfel ) , @kwotaPortfela, @data, @opis)", conn);
            cmd.Parameters.AddWithValue("@kwotaPortfela", wartosc);
            cmd.Parameters.AddWithValue("Id_portfel", this.IdPortfelGotowkowy);
            cmd.Parameters.AddWithValue("@data", DateTime.Now);
            cmd.Parameters.AddWithValue("@opis", opis);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        //Jeżeli chcecie wprowadzić zmiene dla portfela gotówkowego.
        //Wartosc- dodatnia = dodaniesz do portfela, ujemna= odejmujesz od portfela
        //Opis - podaj odrazu opis który chcesz wprowadzić dla historii
        public void ZmianaWartości(decimal wartosc, string opis)
        {
            using var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();

            var cmd = new NpgsqlCommand("UPDATE \"Portfel Gotówkowy\" SET \"Kwota\"=\"Kwota\"+@kwota " +
                "WHERE \"Id_Portfela_Gotówkowego\"=@Id_portfel", conn);
            cmd.Parameters.AddWithValue("@kwota", wartosc);
            cmd.Parameters.AddWithValue("@Id_portfel", this.IdPortfelGotowkowy);
            cmd.ExecuteNonQuery();
            conn.Close();
            this.Wartosc += wartosc;
            TworzenieHistori(wartosc, opis);
        }
    }

}
