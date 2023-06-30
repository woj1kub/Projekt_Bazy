using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
    /// Logika interakcji dla klasy KontoOszczednosciowe.xaml
    /// </summary>
    public partial class KontoOszczednosciowePanel : UserControl
    {
        private readonly string ActiveUser;
        ObservableCollection<Portfel> portfele = new();
        ObservableCollection<KontoOszczędnościowe> konta = new();

        public KontoOszczednosciowePanel(string activeUser)
        {
            ActiveUser = activeUser;
            InitializeComponent();
            PobierzPortfele();
        }
        private void btDodajKonto_Click(object sender, RoutedEventArgs e)
        {
            if (cbPortfele.SelectedIndex == -1 || cbPortfeleGotowkowe.SelectedIndex == -1) return;
            
            int index = cbPortfele.SelectedIndex;
            Portfel p = portfele[index];
            decimal kwotaDodawana = 0;
            KontoOszczędnościowe k=null;
            try
            {
               kwotaDodawana = decimal.Parse(txtKwota.Text);
               k = new(DateTime.Now, kwotaDodawana, double.Parse(txtOprocentowanie.Text),
               dpDataWyplatyOdsetek.SelectedDate, double.Parse(txtPodatek.Text), txtNazwa.Text, dpDataWyplatyOdsetek.SelectedDate);
            }
            catch { MessageBox.Show("Input not in correct format"); return; }

            k.AddToDatabase(p);
            PrzelewanieSrodkowPortfelGotowkowy(kwotaDodawana,false);
            refreshDataKonta();
        }

        private void btnUsun_Click(object sender, RoutedEventArgs e)
        {
            if (cbWybierzKonto.SelectedIndex == -1) return;

            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd = new("DELETE FROM \"Konto oszczędnościowe\" WHERE \"Id_Konta_Oszczędnościowego\" = @idkonta");
            int index = cbWybierzKonto.SelectedIndex;
            KontoOszczędnościowe k = konta[index];
            cmd.Parameters.AddWithValue("@idkonta", k.Id_KontaOszczędnościowego);
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();
            conn.Close();
            PrzelewanieSrodkowPortfelGotowkowy(k.Kwota,true);
            refreshDataKonta();
        }

        private void btnWplataDodanie_Click(object sender, RoutedEventArgs e)
        {
            if (cbWybierzKonto.SelectedIndex == -1) return;
            decimal kwotaZmieniana = 0;
            try { kwotaZmieniana = decimal.Parse(txtWplataDodanie.Text); }
            catch { MessageBox.Show("Input not in correct format");return; }
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd = new($"SELECT * FROM \"Konto oszczędnościowe\" WHERE \"Id_Konta_Oszczędnościowego\" = @idkonta");
            int index = cbWybierzKonto.SelectedIndex;
            KontoOszczędnościowe k = konta[index];
            cmd.Parameters.AddWithValue("@idkonta", k.Id_KontaOszczędnościowego);
            cmd.Connection = conn;
            NpgsqlDataReader reader = cmd.ExecuteReader();
            decimal kwotaStara = 0, kwotaNowa = 0;
            while (reader.Read())
            {
                kwotaStara = reader.GetDecimal(3);
            }
            conn.Close();
            kwotaNowa = kwotaStara + kwotaZmieniana;

            var conn2 = new NpgsqlConnection(Registration.ConnString());
            conn2.Open();
            NpgsqlCommand cmd2 = new($"UPDATE \"Konto oszczędnościowe\" SET \"Kwota\" = @kwota WHERE \"Id_Konta_Oszczędnościowego\" = @idkonta");
            cmd2.Parameters.AddWithValue("@idkonta", k.Id_KontaOszczędnościowego);
            cmd2.Parameters.AddWithValue("@kwota", kwotaNowa);
            cmd2.Connection = conn2;
            cmd2.ExecuteNonQuery();
            conn2.Close();
            PrzelewanieSrodkowPortfelGotowkowy(kwotaZmieniana, false);
            refreshDataKonta();
            }

        private void PrzelewanieSrodkowPortfelGotowkowy(decimal kwotaZmieniana,bool plus)
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
            if(plus)
                kwotaNowa = kwotaStara + kwotaZmieniana;
            else
                kwotaNowa = kwotaStara - kwotaZmieniana;

            var conn2 = new NpgsqlConnection(Registration.ConnString());
            conn2.Open();
            NpgsqlCommand cmd2 = new($"UPDATE \"Portfel Gotówkowy\" SET \"Kwota\" = @kwota WHERE \"Id_Portfela_Gotówkowego\" = @idportfel");
            cmd2.Parameters.AddWithValue("@idportfel", pg.IdPortfelGotowkowy);
            cmd2.Parameters.AddWithValue("@kwota", kwotaNowa);
            cmd2.Connection = conn2;
            cmd2.ExecuteNonQuery();
            conn2.Close();

            object semafor=null;
            SelectionChangedEventArgs a=null;
            cbPortfele_SelectionChanged(semafor,a);
        }

        private void PobierzPortfele()
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
                portfele.Add(portfel);
                cbPortfele.Items.Add(portfel.Nazwa);

            }
            conn.Close();
        }

        private void refreshDataKonta()
        {
            cbWybierzKonto.Items.Clear();
            konta = null;
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd = new($"SELECT * FROM \"Konto oszczędnościowe\" WHERE \"Id_Portfelu\" = @idportfel");
            int index = cbPortfele.SelectedIndex;
            Portfel p = portfele[index];
            cmd.Parameters.AddWithValue("@idportfel", p.PortfeleId);
            cmd.Connection = conn;
            konta = new();
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
                };
                konta.Add(kontoOszczędnościowe);
                cbWybierzKonto.Items.Add(kontoOszczędnościowe.Nazwa);
            }
            conn.Close();
        }

        private void cbPortfele_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbPortfeleGotowkowe.Items.Clear();
            refreshDataKonta();
            int index = cbPortfele.SelectedIndex;
            Portfel p = portfele[index];
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd = new($"SELECT * FROM \"Portfel Gotówkowy\" WHERE \"Id_Porfelu\" = @idportfel");
            cmd.Parameters.AddWithValue("@idportfel", p.PortfeleId);
            cmd.Connection = conn;
            NpgsqlDataReader reader = cmd.ExecuteReader();
            PortfelGotówkowy pg;
            while (reader.Read())
            {
                pg = new()
                {
                    IdPortfelGotowkowy = reader.GetInt64(0),
                    Wartosc = reader.GetDecimal(2)
                };
                cbPortfeleGotowkowe.Items.Add(pg);
            }
            conn.Close();
            cbPortfeleGotowkowe.SelectedIndex = 0;
        }

        private void cbWybierzKonto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbWybierzKonto.HasItems)
            {
                lblTymczasoweSzczegoly.Content = "";
                int index = cbWybierzKonto.SelectedIndex;
                KontoOszczędnościowe k = konta[index];
                lblTymczasoweSzczegoly.Content=k.ToString();
            }
        }

        private void cbPortfeleGotowkowe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //PortfelGotówkowy pg = cbPortfeleGotowkowe.SelectedItem as PortfelGotówkowy;
            //MessageBox.Show(pg.IdPortfelGotowkowy.ToString() + " " + pg.Wartosc.ToString());
        }
    }
}
