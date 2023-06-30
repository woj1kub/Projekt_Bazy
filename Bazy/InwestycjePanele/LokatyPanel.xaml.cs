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
    /// Logika interakcji dla klasy Lokaty.xaml
    /// </summary>
    public partial class LokatyPanel : UserControl
    {
        private readonly string ActiveUser;
        ObservableCollection<Portfel> portfele = new();
        ObservableCollection<KontoOszczędnościowe> lokaty = new();

        string nazwaLokaty = "";
        decimal kwota = 0;
        float oprocentowanie = 0;
        DateTime data_zalozenia = DateTime.Now;
        DateTime data_zakonczenia = DateTime.Now;

        public LokatyPanel(string activeUser)
        {
            ActiveUser = activeUser;
            InitializeComponent();
            PobierzPortfele();
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
                cbWybierzPortfel.Items.Add(portfel.Nazwa);

            }
            conn.Close();
        }
        private void btDodajLokate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cbWybierzPortfel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbPortfeleGotowkowe.Items.Clear();
            //refreshDataKonta();
            int index = cbWybierzPortfel.SelectedIndex;
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
    }
}
