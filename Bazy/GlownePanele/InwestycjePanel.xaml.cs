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
    /// Logika interakcji dla klasy UserControl1.xaml
    /// </summary>
    public partial class InwestycjePanel : UserControl
    {
        ObservableCollection<Portfel> portfels = new();
        Action<ObservableCollection<Portfel>> ActivePortfel;

        private readonly string ActiveUser="";
        
        public InwestycjePanel(ObservableCollection<Portfel> portfels, Action<ObservableCollection<Portfel>> ActivePortfel, string ActiveUser) 
        {
            this.ActiveUser = ActiveUser;
            this.portfels= portfels;
            this.ActivePortfel += ActivePortfel;
            InitializeComponent();
        }
        ~InwestycjePanel()
        {
            ActivePortfel.Invoke(portfels);
            GC.Collect();
        }
        private void btnLokaty_Click(object sender, RoutedEventArgs e)
        {
            LokatyPanel lokaty = new(portfels, ActivePortfel);
            oknoInwestycje.Content = lokaty;
        }

        private void btnKontoOszczednosciowe_Click(object sender, RoutedEventArgs e)
        {
            KontoOszczednosciowePanel kontoOszczednosciowe = new(ActiveUser);
            oknoInwestycje.Content = kontoOszczednosciowe;
        }

        private void btnObligacje_Click(object sender, RoutedEventArgs e)
        {
            ObligacjePanel obligacje = new();
            oknoInwestycje.Content = obligacje;
        }

        private void btnAkcje_Click(object sender, RoutedEventArgs e)
        {
            AkcjePanel akcje = new();
            oknoInwestycje.Content = akcje;
        }
    }
}
