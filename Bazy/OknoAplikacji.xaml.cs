
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bazy
{
    /// <summary>
    /// Logika interakcji dla klasy OknoAplikacji.xaml
    /// </summary>
    public partial class OknoAplikacji : Window
    {
        public OknoAplikacji()
        {
            InitializeComponent();
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btInwestycje_Click(object sender, RoutedEventArgs e)
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
        }
    }
}
