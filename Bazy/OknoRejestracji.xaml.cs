
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

        private void brHaslo_MouseEnter(object sender, MouseEventArgs e)
        {

            textHaslo.Visibility = Visibility.Hidden;
        }

        private void brHaslo_MouseLeave(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(txtHaslo.Password))
                textHaslo.Visibility = Visibility.Visible;
        }

        private void brLogin_MouseEnter(object sender, MouseEventArgs e)
        {
            textLogin.Visibility = Visibility.Hidden;
        }

        private void brLogin_MouseLeave(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(txtLogin.Text))
                textLogin.Visibility = Visibility.Visible;
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            textPotwierdzHaslo.Visibility = Visibility.Hidden;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPotwierdzHaslo.Password))
                textPotwierdzHaslo.Visibility = Visibility.Visible;
        }

        private void txtLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtLogin.Text) || txtLogin.Text.Length > 0)
            {
                textLogin.Visibility = Visibility.Collapsed;
            }
            else
            {
                textLogin.Visibility = Visibility.Visible;
            }
        }


        private void txtHaslo_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtHaslo.Password) || txtHaslo.Password.Length > 0)
            {
                textHaslo.Visibility = Visibility.Collapsed;
            }
            else
            {
                textHaslo.Visibility = Visibility.Visible;
            }
        }

        private void txtPotwierdzHaslo_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPotwierdzHaslo.Password) || txtPotwierdzHaslo.Password.Length > 0)
            {
                textPotwierdzHaslo.Visibility = Visibility.Collapsed;
            }
            else
            {
                textPotwierdzHaslo.Visibility = Visibility.Visible;
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
                lbErrorRejestraji.Visibility = Visibility.Visible;
                lbErrorRejestraji.Content = "Błąd wprowadzonych danych";
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
