
using System.Collections.Generic;
using System;
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
        protected string ActiveUser;
       
        public OknoAplikacji(string ActiveUser)
        {
            InitializeComponent();
            this.ActiveUser = ActiveUser;

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

        private void btWyloguj_Click(object sender, RoutedEventArgs e)
        {
            var okno = new MainWindow();
            this.Close();
            okno.Show();
        }

    }

   
}
