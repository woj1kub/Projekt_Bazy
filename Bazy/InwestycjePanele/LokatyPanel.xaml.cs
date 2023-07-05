using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Bazy
{
    /// <summary>
    /// Logika interakcji dla klasy Lokaty.xaml
    /// </summary>
    public partial class LokatyPanel : UserControl
    {
        ObservableCollection<Portfel> portfels = new();
        Action<ObservableCollection<Portfel>> ActivePortfel;
        ObservableCollection<String> nazwa = new();

        string nazwaLokaty = "";
        decimal kwota = 0;
        float oprocentowanie = 0;
        DateTime data_zalozenia = DateTime.Now;
        DateTime data_zakonczenia = DateTime.Now;

        public LokatyPanel(ObservableCollection<Portfel> portfels, Action<ObservableCollection<Portfel>> ActivePortfel)
        {
            this.portfels = portfels;
            this.ActivePortfel += ActivePortfel;
            InitializeComponent();
            cbWybierzPortfel.ItemsSource = portfels;
            cbPortfel.ItemsSource = portfels;
            //cbKapitalizacja.Items.Add(kapitalizacjaOdsetek.Jednorazowa);
            //cbKapitalizacja.Items.Add(kapitalizacjaOdsetek.Roczna);
            //cbKapitalizacja.Items.Add(kapitalizacjaOdsetek.Miesieczna);
            //cbKapitalizacja.Items.Add(kapitalizacjaOdsetek.Dzienna);
            dbDataZalozenia.SelectedDate = DateTime.Now;
            dbDataZakonczenia.SelectedDate = DateTime.Now.AddDays(7);
        }

        private void btDodajLokate_Click(object sender, RoutedEventArgs e)
        {
            var index = cbWybierzPortfel.SelectedIndex; if (index == -1) { return; }
            long Id_portfelu = portfels[index].PortfeleId;
            Lokaty nowaLokata = new Lokaty
            {
                Oprocentowanie = double.Parse(txtOprocentowanie.Text),
                Kwota = decimal.Parse(txtKwota.Text),
                Podatek = 19, // Ustaw podatek na wartość domyślną
                Data_zakupu = dbDataZalozenia.SelectedDate.Value,
                Nazwa = txtNazwaLokaty.Text,
                Data_zakończenia = dbDataZakonczenia.SelectedDate.Value,
                //Kapitalizacjaodesetek = (kapitalizacjaOdsetek)cbKapitalizacja.SelectedIndex
            };

            using (var conn = new NpgsqlConnection(Registration.ConnString()))
            {
                conn.Open();
                string Polecanie = "INSERT INTO \"Lokaty\" (\"Id_Portefelu\",\"Oprocentowanie\", \"Kwota\", \"Podatek\", \"Data_Zakupu\", \"Nazwa\", \"Data_Zakończenia\") " +
                "VALUES (@idportfelu, @oprocentowanie, @kwota, @podatek, @datazakupu, @nazwa, @datazakonczenia )";
                var cmd = new NpgsqlCommand(Polecanie, conn);
                cmd.Parameters.AddWithValue("@idportfelu", Id_portfelu);
                cmd.Parameters.AddWithValue("@oprocentowanie", nowaLokata.Oprocentowanie);
                cmd.Parameters.AddWithValue("@kwota", nowaLokata.Kwota);
                cmd.Parameters.AddWithValue("@podatek", nowaLokata.Podatek);
                cmd.Parameters.AddWithValue("@datazakupu", nowaLokata.Data_zakupu);
                cmd.Parameters.AddWithValue("@nazwa", nowaLokata.Nazwa);
                cmd.Parameters.AddWithValue("@datazakonczenia", nowaLokata.Data_zakończenia);
                //cmd.Parameters.AddWithValue("@kapitalizacja", (int)nowaLokata.Kapitalizacjaodesetek);
                cmd.Connection = conn;
                cmd.ExecuteScalar();

                conn.Close();
            }
            decimal kwotaZmieniana = 0;
            try { kwotaZmieniana = decimal.Parse(txtKwota.Text); }
            catch { MessageBox.Show("Input not in correct format"); return; }
            PrzelewanieSrodkowPortfelGotowkowy(kwotaZmieniana, false);

        }

        private void PrzelewanieSrodkowPortfelGotowkowy(decimal kwotaZmieniana, bool plus)
        {
            PortfelGotówkowy pg = cbPortfeleGotowkowe.SelectedItem as PortfelGotówkowy;

            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd = new($"SELECT * FROM \"Portfel Gotówkowy\" WHERE \"Id_Portfela_Gotówkowego\" = @idportfel");
            cmd.Parameters.AddWithValue("@idportfel", pg.IdPortfelGotowkowy);
            cmd.Connection = conn;
            NpgsqlDataReader reader = cmd.ExecuteReader();
            decimal kwotaStara = 0, kwotaNowa = 0;
            while (reader.Read())
            {
                kwotaStara = reader.GetDecimal(2);
            }
            conn.Close();
            
            kwotaNowa = kwotaStara - kwotaZmieniana;

            var conn2 = new NpgsqlConnection(Registration.ConnString());
            conn2.Open();
            NpgsqlCommand cmd2 = new($"UPDATE \"Portfel Gotówkowy\" SET \"Kwota\" = @kwota WHERE \"Id_Portfela_Gotówkowego\" = @idportfel");
            cmd2.Parameters.AddWithValue("@idportfel", pg.IdPortfelGotowkowy);
            cmd2.Parameters.AddWithValue("@kwota", kwotaNowa);
            cmd2.Connection = conn2;
            cmd2.ExecuteNonQuery();
            conn2.Close();

            object semafor = null;
            SelectionChangedEventArgs a = null;
            cbWybierzPortfel_SelectionChanged(semafor, a);
            cbPortfel_SelectionChanged(semafor, a);
        }

        private void usunALL()
        {
            using (var conn = new NpgsqlConnection(Registration.ConnString()))
            {
                conn.Open();
                string Polecanie = "DELETE FROM Lokaty";
                var cmd = new NpgsqlCommand(Polecanie, conn);
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        public class ListaLokat
        {
            public String NazwaLok { get; set; }
            public long IdLokaty { get; set; }
        }
        ObservableCollection<ListaLokat> lokatyLista = new();
        private void wypelnijListe()
        {
            var index = cbPortfel.SelectedIndex; if (index == -1) { return; }
            long Id_portfelu = portfels[index].PortfeleId;
            using (var conn = new NpgsqlConnection(Registration.ConnString()))
            {
                conn.Open();
                string Polecanie = $"SELECT \"Nazwa\", \"Id_Lokaty\" FROM \"Lokaty\" WHERE \"Id_Portefelu\" = @idportfelu";
                var cmd = new NpgsqlCommand(Polecanie, conn);
                cmd.Parameters.AddWithValue("@idportfelu", Id_portfelu);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //nazwa_lokaty.Add(new NazwaLokaty { NazwaLok = reader.GetString(0)});
                        // cbLokata.Items.Add(reader.GetString(0));
                        ListaLokat newLokata = new ListaLokat();
                        newLokata.NazwaLok = reader.GetString(0);
                        newLokata.IdLokaty = reader.GetInt64(1);
                        lokatyLista.Add(newLokata);
                    }
                }
                conn.Close();
            }
            cbLokata.ItemsSource = lokatyLista;
            cbLokata.DisplayMemberPath = "NazwaLok";
            //ltvLokaty.ItemsSource = nazwa_lokaty;
        }
        private void cbWybierzPortfel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index=cbWybierzPortfel.SelectedIndex; if (index == -1) { return; }
            cbPortfeleGotowkowe.ItemsSource = portfels[index].PortfeleGotówkowe;
        }

        ~LokatyPanel()
        {
            ActivePortfel.Invoke(portfels);
            GC.Collect();
        }

        private void cbPortfeleGotowkowe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void txtKwota_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private void txtNazwaLokaty_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private void txtOprocentowanie_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private void btnOblicz_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void cbPortfel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = cbPortfel.SelectedIndex; if (index == -1) { return; }
            cbPortfeleGotowkowe.ItemsSource = portfels[index].PortfeleGotówkowe;
            wypelnijListe();
        }

        private void cbLokata_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Lokaty nowa = new Lokaty();
            var index = cbLokata.SelectedIndex; if (index == -1) { return; }
            long idlokaty = lokatyLista[index].IdLokaty;
            using (var conn = new NpgsqlConnection(Registration.ConnString()))
            {
                conn.Open();
                string Polecanie = $"SELECT * FROM \"Lokaty\" WHERE \"Id_Lokaty\" = @idlokaty";
                var cmd = new NpgsqlCommand(Polecanie, conn);
                cmd.Parameters.AddWithValue("@idlokaty", idlokaty);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        nowa.Id_Lokaty = reader.GetInt64(0);
                        nowa.Oprocentowanie = reader.GetDouble(2);
                        nowa.Kwota = reader.GetDecimal(3);
                        nowa.Podatek = reader.GetDouble(4);
                        nowa.Data_zakupu = reader.GetDateTime(5);
                        nowa.Nazwa = reader.GetString(6);
                        nowa.Data_zakończenia = reader.GetDateTime(7);
                        nowa.Kapitalizacjaodesetek = (kapitalizacjaOdsetek)2;
                    }
                }
                conn.Close();
            }

            decimal wynik = nowa.ObliczZysk();

            lblKwotaLokaty.Content =nowa.Kwota;
            lblRozpoczecieLokaty.Content =nowa.Data_zakupu;
            lblZakonczenieLokaty.Content =nowa.Data_zakończenia;
            lblZyskLokaty.ContentStringFormat = "{0:0.00}";
            lblZyskLokaty.Content =  wynik;
            lblOprocentowanieLokaty.Content =  nowa.Oprocentowanie;
        }
    }
}
