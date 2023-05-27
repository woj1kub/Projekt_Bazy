﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
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
using Npgsql;

namespace Bazy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //test_hasla();
            //Registration.AllUsersShow();
            //Registration.DeleteAllUsers();
            InitializeComponent();
        }
        void test_hasla()
        {
            string test_psd = "ZAQ!1qaz";
            byte[] user_salt;
            string hash = PasswordInterface.HashPasword(test_psd, out user_salt);

            MessageBox.Show(PasswordInterface.VerifyPassword(test_psd, hash, user_salt).ToString());
        }

        private void textLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            textLogin.Visibility = Visibility.Collapsed;
        }

        private void txtLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(string.IsNullOrEmpty(txtLogin.Text) && txtLogin.Text.Length>0)
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

        private void btnZaloguj_Click(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrEmpty(txtLogin.Text) && !string.IsNullOrEmpty(txtHaslo.Password))
            {
                
                if (VerifyUserExist(txtLogin.Text, txtHaslo.Password))
                {
                MessageBox.Show("Zalogowano");
                var okno = new OknoAplikacji();
                this.Close();
                okno.Show();
                }
                else
                    MessageBox.Show("Złe hasło lub login");
            }
            else
            {
                MessageBox.Show("Wypełnij dane");
            }

        }
        private bool VerifyUserExist(string login, string password)
        {
            byte[] Salt=null;
            String Password_base=null;
            using (var conn = new NpgsqlConnection(Registration.ConnString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM \"Użytkownicy\" WHERE \"Login\"=@loginBase ");
                cmd.Parameters.AddWithValue("@loginBase", login);
                
                cmd.Connection = conn;
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Password_base = reader.GetString(1);
                    Salt = (byte[]?) reader.GetValue(2);
                }
            }
            if (Password_base == null || Salt==null)
                return false;

            return PasswordInterface.VerifyPassword(password, Password_base, Salt);

        }



        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnZarejestruj_Click(object sender, RoutedEventArgs e)
        {
            //temp stuff here
            //Registration.RegisterUser(txtLogin.Text, txtHaslo.Password);
            //Registration.AllUsersShow();

            var okno = new OknoRejestracji();
            this.Close();
            okno.ShowDialog();
            


        }
    }
}
