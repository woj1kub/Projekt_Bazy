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
    public partial class Inwestycje : UserControl
    {
        public Inwestycje()
        {
            InitializeComponent();
        }

        private void btnLokaty_Click(object sender, RoutedEventArgs e)
        {
            Lokaty lokaty = new Lokaty();
            oknoInwestycje.Content = lokaty;
        }

        private void btnKontoOszczednosciowe_Click(object sender, RoutedEventArgs e)
        {
            KontoOszczednosciowe kontoOszczednosciowe = new KontoOszczednosciowe();
            oknoInwestycje.Content = kontoOszczednosciowe;
        }
    }
}
