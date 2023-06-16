using System;
using System.Collections.Generic;
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
        public InwestycjePanel()
        {
            InitializeComponent();
        }

        private void btnLokaty_Click(object sender, RoutedEventArgs e)
        {
            LokatyPanel lokaty = new();
            oknoInwestycje.Content = lokaty;
        }

        private void btnKontoOszczednosciowe_Click(object sender, RoutedEventArgs e)
        {
            KontoOszczednosciowePanel kontoOszczednosciowe = new();
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
