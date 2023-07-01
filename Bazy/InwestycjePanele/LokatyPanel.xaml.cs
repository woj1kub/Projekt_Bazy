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

        }

        private void btDodajLokate_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
