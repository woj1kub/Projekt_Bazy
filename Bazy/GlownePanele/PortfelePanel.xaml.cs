using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Bazy
{
    public partial class PortfelePanel : UserControl
    {
        private readonly string ActiveUser;
        ObservableCollection<Portfel> portfele_dane = new();
        event Action<ObservableCollection<Portfel>> ActivePortfel;
        Portfel portfel_wew=new();

        public PortfelePanel(string ActiveUser, ObservableCollection<Portfel> portfels, Action<ObservableCollection<Portfel>> ActivePortfel)
        {
            portfele_dane = portfels;
            InitializeComponent();
            this.ActiveUser = ActiveUser;
            this.ActivePortfel += ActivePortfel;
            lbiPortfele.ItemsSource = portfele_dane;
        }

        ~PortfelePanel()
        {
            ActivePortfel.Invoke(portfele_dane);
            GC.Collect();
        }

        private void lbiPortfele_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int chosen = lbiPortfele.SelectedIndex;
            if (chosen < 0)
            {
                if (ActivePortfel == null)
                {
                    return;
                }
                chosen = portfele_dane.IndexOf(portfel_wew);
                if (chosen < 0)
                {
                    return;
                }
            }
            portfel_wew = portfele_dane[chosen];
            lbiPortfele.SelectedIndex = chosen;
            lbPortfeleGotówkowe.ItemsSource = portfel_wew.portfeleGotówkowe;
        }
        
        private void btDodaj_Click(object sender, RoutedEventArgs e)
        {
            if (NewPortfelName.Text == string.Empty)
                return;
            //uzupełnienie danych w tablicy portfeli
            Portfel portfel = new(ActiveUser, NewPortfelName.Text);

            portfele_dane.Add(portfel);

            //Czyszczenie
            NewPortfelName.Clear();
            //Sortowanie
            portfele_dane = new ObservableCollection<Portfel>(portfele_dane.OrderByDescending(item => item.Wartosc));
            lbiPortfele.ItemsSource = portfele_dane;

        }

        private void btUsuń_Click(object sender, RoutedEventArgs e)
        {
            if (portfel_wew == null || !portfele_dane.Contains(portfel_wew) ) return;
            
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd;

            cmd = new("DELETE FROM \"Portfele\" WHERE \"Id_Portfelu\"= @Id_portfel");
            cmd.Parameters.AddWithValue("Id_portfel", portfel_wew.PortfeleId);
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();
            conn.Close();
            portfele_dane.RemoveAt(portfele_dane.IndexOf(portfel_wew));

            lbiPortfele.SelectedIndex = -1;
            lbPortfeleGotówkowe.ItemsSource=null;

        }

        private void btDodajPortfelGotowkowy_Click(object sender, RoutedEventArgs e)
        {
            if (portfel_wew == null || PortfelGotowkowy.Text==string.Empty || !portfele_dane.Contains(portfel_wew)) return;

            decimal wartosc = decimal.Parse(PortfelGotowkowy.Text);

            portfele_dane = new ObservableCollection<Portfel>(portfele_dane.OrderByDescending(item => item.Wartosc));
            lbiPortfele.ItemsSource = portfele_dane;
            portfel_wew.portfeleGotówkowe.Add(new PortfelGotówkowy(wartosc: wartosc , idPorfela:(long) portfel_wew.PortfeleId, portfel_wew.Nazwa));
            PortfelGotowkowy.Clear();
            RestartPortfela();

        }

        private void _PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1) && e.Text != ",")
            {
                e.Handled = true; 
            }
            string newText = ((TextBox) sender).Text + e.Text;
            Regex regex = new(@"^\d+(,\d{0,2})?$");
            if (!regex.IsMatch(newText))
            {
                e.Handled = true;
            }
        }

        private void DeletePG_Click(object sender, RoutedEventArgs e)
        {
            

        }

        private void btDodajFundusze_Click(object sender, RoutedEventArgs e)
        {
            if (lbPortfeleGotówkowe.SelectedIndex==-1 || portfel_wew.portfeleGotówkowe == null || portfel_wew == null || DodFundusze.Text == string.Empty || !portfele_dane.Contains(portfel_wew)) return;
            decimal wartosc = decimal.Parse(DodFundusze.Text);
            portfel_wew.portfeleGotówkowe[lbPortfeleGotówkowe.SelectedIndex].ZmianaWartości(wartosc, "Wpłacenie do portfela gotówkowegoz dla" + portfel_wew.Nazwa);
            
            var selectedPortfel = portfel_wew.portfeleGotówkowe[lbPortfeleGotówkowe.SelectedIndex];
            portfel_wew.portfeleGotówkowe[lbPortfeleGotówkowe.SelectedIndex] = new(selectedPortfel);
            portfel_wew.Wartosc += wartosc;
            DodFundusze.Text = "";
            RestartPortfela();
        }

        void RestartPortfela() 
        {
            var selectedIndex = portfele_dane.IndexOf(portfel_wew);
            var selectedPortfel = portfele_dane[selectedIndex];
            portfele_dane[selectedIndex] = new(selectedPortfel);
            lbiPortfele.SelectedIndex = selectedIndex;
        }

        private void btUsunFundusze_Click(object sender, RoutedEventArgs e)
        {
            if (lbPortfeleGotówkowe.SelectedIndex == -1 || portfel_wew.portfeleGotówkowe == null || portfel_wew == null || UsuFundusze.Text == string.Empty || !portfele_dane.Contains(portfel_wew)) return;
            decimal wartosc = decimal.Parse(UsuFundusze.Text);
            portfel_wew.portfeleGotówkowe[lbPortfeleGotówkowe.SelectedIndex].ZmianaWartości(-wartosc, "Pobranie z portfela gotówkowego z " + portfel_wew.Nazwa);

            var selectedPortfel = portfel_wew.portfeleGotówkowe[lbPortfeleGotówkowe.SelectedIndex];
            portfel_wew.portfeleGotówkowe[lbPortfeleGotówkowe.SelectedIndex] = new(selectedPortfel);
            portfel_wew.Wartosc -= wartosc;
            UsuFundusze.Text = "";

            RestartPortfela();
        }
    }
}
