
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;

namespace Bazy
{
    /// <summary>
    /// Logika interakcji dla klasy OknoRejestracji.xaml
    /// </summary>
    public partial class OknoRejestracji : Window
    {
        public OknoRejestracji()
        {
            InitializeComponent();
        }

        private void textLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            textLogin.Visibility = Visibility.Collapsed;
        }

        private void txtLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtLogin.Text) && txtLogin.Text.Length > 0)
            {
                txtLogin.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtLogin.Visibility = Visibility.Visible;
            }
        }

        private void textHaslo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            textHaslo.Visibility = Visibility.Collapsed;
        }

        private void txtHaslo_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtHaslo.Password) && txtHaslo.Password.Length > 0)
            {
                txtHaslo.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtHaslo.Visibility = Visibility.Visible;
            }
        }

        private void textPotwierdzHaslo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            textPotwierdzHaslo.Visibility = Visibility.Collapsed;
        }

        private void txtPotwierdzHaslo_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPotwierdzHaslo.Password) && txtPotwierdzHaslo.Password.Length > 0)
            {
                txtPotwierdzHaslo.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtPotwierdzHaslo.Visibility = Visibility.Visible;
            }
        }

        private void btnZarejestruj_Click(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrEmpty(txtLogin.Text) && !string.IsNullOrEmpty(txtHaslo.Password) &&
                Registration.RegisterUser(txtLogin.Text, txtHaslo.Password, txtPotwierdzHaslo.Password))
            {
                MessageBox.Show("Zarejestrowano pomyślnie.");
                var okno = new MainWindow();
                this.Close();
                okno.Show();
            }
            else
            {
                MessageBox.Show("Wypełnij dane");
            }

        }

        private void btnZaloguj_Click(object sender, RoutedEventArgs e)
        {
            var okno = new MainWindow();
            this.Close();
            okno.Show();
            
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
