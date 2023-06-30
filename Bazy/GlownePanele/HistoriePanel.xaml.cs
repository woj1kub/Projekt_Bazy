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
    /// Logika interakcji dla klasy Historie.xaml
    /// </summary>
    public partial class HistoriePanel : UserControl
    {
        public HistoriePanel()
        {
            InitializeComponent();
        }


        public class Historia
        {
            public string Opis { get; set; }
            public decimal Kwota { get; set; }
            public DateTime Data { get; set; }
        }



        public void UzupelnijHistorie()
        {
            var historia = new ObservableCollection<Historia>();

            using (var conn = new NpgsqlConnection(Registration.ConnString()))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("SELECT \"Opis_Transakcji\", \"Kwota\", \"Data_Transakcji\" FROM \"Historia Transakcji Portfelu\" ORDER BY \"Data_Transakcji\" DESC", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        historia.Add(new Historia { Opis = reader.GetString(0), Kwota = reader.GetDecimal(1), Data = reader.GetDateTime(2) });
                    }
                }
            }

            ltvHistoria.ItemsSource = historia;
        }
    }
}
