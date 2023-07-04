using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using OxyPlot.Axes;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Linq;

namespace Bazy
{
    /// <summary>
    /// Logika interakcji dla klasy Historie.xaml
    /// </summary>
    public partial class HistoriePanel : UserControl
    {
        public class Historia
        {
            public string Opis { get; set; }
            public decimal Kwota { get; set; }
            public DateTime Data { get; set; }
        }

        ObservableCollection<Historia> historia = new();
        ObservableCollection<Portfel> portfels=new();
        public HistoriePanel(ObservableCollection<Portfel> portfelGotówkowies)
        {
            this.portfels = portfelGotówkowies;
            InitializeComponent();
            UzupelnijHistorie();
            
        }

        void TworzenieWykresu()
        {
            List<Historia> histrias = historia.OrderBy(x => x.Data).ToList();
            PlotModel Wykres = new PlotModel { Title = "Wykres działalności na portfelach gotówkowych"};
            var seriaDanych = new LineSeries();
            seriaDanych.MarkerType = MarkerType.Circle;
            seriaDanych.MarkerSize = 2;
            seriaDanych.Color = OxyColors.Blue;
            double wartosc = 0;
            foreach (var item in histrias) 
            {
                wartosc += Convert.ToDouble(item.Kwota);
                seriaDanych.Points.Add(new DataPoint(DateTimeAxis.ToDouble(item.Data), wartosc));
            }
            var linearAxis = new LinearAxis { Position = AxisPosition.Left };
            var dateTimeAxis = new DateTimeAxis { Position = AxisPosition.Bottom };
            Wykres.Axes.Add(linearAxis);
            Wykres.Axes.Add(dateTimeAxis);

            seriaDanych.XAxisKey = dateTimeAxis.Key;
            seriaDanych.YAxisKey = linearAxis.Key;

            Wykres.Series.Add(seriaDanych);
            PVWykres.Model=Wykres;
            
        }


        void UzupelnijHistorie()
        {
            List<string> strings = new List<string>();
            int i = 0;
            foreach (var item in portfels)
            {
                foreach (var item1 in portfels[i].portfeleGotówkowe)
                {
                    strings.Add(item1.IdPortfelGotowkowy.ToString());
                }
                i++;
            }

            using (var conn = new NpgsqlConnection(Registration.ConnString()))
            {
                conn.Open();
                string paramPlaceholders = string.Join(",", strings.Select((_, index) => $"@param{index}"));
                string Polecanie = $"SELECT \"Opis_Transakcji\", \"Kwota\", \"Data_Transakcji\" FROM \"Historia Transakcji Portfelu\" WHERE \"Id_Portfela_Gotówkowego\" IN ({paramPlaceholders}) ORDER BY \"Data_Transakcji\" DESC";
                var cmd = new NpgsqlCommand(Polecanie, conn);
                for (int x = 0; x < strings.Count; x++)
                {
                    cmd.Parameters.AddWithValue($"@param{x}", strings[x]);
                }

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        historia.Add(new Historia { Opis = reader.GetString(0), Kwota = reader.GetDecimal(1), Data = reader.GetDateTime(2) });
                    }
                }
                conn.Close();
            }

            ltvHistoria.ItemsSource = historia;
            TworzenieWykresu();
        }
    }
}
