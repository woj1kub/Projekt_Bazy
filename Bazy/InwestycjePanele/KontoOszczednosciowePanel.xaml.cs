using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            refreshDataPortfele();
        }
        private void btDodajKonto_Click(object sender, RoutedEventArgs e)
        {
            //napisać sprawdzanie czy pola mają poprawne wartości
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd = new("INSERT INTO \"Konto oszczędnościowe\" (\"Id_Portfelu\", \"Data_Założenia\"" +
                ", \"Kwota\", \"Oprocentowanie\", \"Data_Wypłaty_Odsetek\", \"Podatek\",\"Nazwa\") " +
                "VALUES (@Idportfelu, @DataAktualna, @Kwota, @Oprocentowanie, @DataWypłaty, @Podatek, @Nazwa)"
                + "RETURNING \"Id_Konta_Oszczędnościowego\"");

            int index = cbPortfele.SelectedIndex;
            Portfel p = portfele[index];

            cmd.Parameters.AddWithValue("@Idportfelu", p.PortfeleId);
            cmd.Parameters.AddWithValue("@DataAktualna", DateTime.Now);
            cmd.Parameters.AddWithValue("@Kwota", decimal.Parse(txtKwota.Text));
            cmd.Parameters.AddWithValue("@Oprocentowanie", double.Parse(txtOprocentowanie.Text));
            cmd.Parameters.AddWithValue("@DataWypłaty", dpDataWyplatyOdsetek.SelectedDate);
            cmd.Parameters.AddWithValue("@Podatek", double.Parse(txtPodatek.Text));
            cmd.Parameters.AddWithValue("@Nazwa", txtNazwa.Text);
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();
            conn.Close();
            refreshDataKonta();
        }

        private void cbWybierzKonto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            refreshDataKonta();
        }

        private void refreshDataPortfele()
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

        private void btnUsun_Click(object sender, RoutedEventArgs e)
        {
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd = new("DELETE FROM \"Konto oszczędnościowe\"");
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();
            conn.Close();

        }
    }
}
