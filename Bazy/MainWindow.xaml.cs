﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

            InitializeComponent();
            testConnect();
            //addEnum();
            //alterTable();
            checker();
            //deleteTableWithWrongStuffInside();
        }
        private void testConnect()
        {
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            conn.Close();
        }
        
        private void txtLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!string.IsNullOrEmpty(txtLogin.Text) && txtLogin.Text.Length>0)
            {
                textLogin.Visibility = Visibility.Hidden;
            }
            else
            {
                textLogin.Visibility = Visibility.Visible;
            }
        }

        private void txtHaslo_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtHaslo.Password) && txtHaslo.Password.Length > 0)
            {
                textHaslo.Visibility = Visibility.Hidden;
            }
            else
            {
                textHaslo.Visibility = Visibility.Visible;
            }
        }

        private void btnZaloguj_Click(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrEmpty(txtLogin.Text) && !string.IsNullOrEmpty(txtHaslo.Password))
            {
                
                if (VerifyUserExist(txtLogin.Text, txtHaslo.Password))
                {
                //MessageBox.Show("Zalogowano");
                var okno = new OknoAplikacji( txtLogin.Text);
                this.Close();
                okno.Show();
                }
                else
                    //MessageBox.Show("Złe hasło lub login");
                    showLoginMsg("Złe hasło lub login");
            }
            else
            {
                //MessageBox.Show("Wypełnij dane");
                showLoginMsg("Wypełnij dane");
            }

        }
        private bool VerifyUserExist(string login, string password)
        {
            byte[]? Salt=null;
            String? Password_base=null;
            using (var conn = new NpgsqlConnection(Registration.ConnString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new($"SELECT * FROM \"Użytkownicy\" WHERE \"Login\"=@loginBase");
                cmd.Parameters.AddWithValue("@loginBase", login);
                
                cmd.Connection = conn;
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Password_base = reader.GetString(1);
                    Salt = (byte[]) reader.GetValue(2);
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
            var okno = new OknoRejestracji();
            this.Close();
            okno.ShowDialog();
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

        private void showLoginMsg(string msg)
        {
            lbLoginMsg.Content = msg;
        }

        //AlterTable
        void addEnum()
        {
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd = new("CREATE TYPE KapitalizacjaOdsetek AS ENUM (\'Jednorazowa\',\'Roczna\',\'Miesięczna\',\'Dzienna\') ", conn);
            int ss = cmd.ExecuteNonQuery();
            conn.Close();
            MessageBox.Show(ss.ToString());
        }
        void alterTable()
        {
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd = new("ALTER TABLE \"Konto oszczędnościowe\" ADD COLUMN \"Kapitalizacja\" DATE");
            cmd.Connection=conn;
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        void checker()
        {
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd = new("SHOW ENUMS", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            List<string> list = new List<string>();
            while (reader.Read())
            {
                var asa = reader.GetValue(2);
                Console.WriteLine(asa);
                asa = null;

            }
            conn.Close();
        }
        void deleteTableWithWrongStuffInside() 
        {
            var conn = new NpgsqlConnection(Registration.ConnString());
            conn.Open();
            NpgsqlCommand cmd = new("DELETE FROM \"Lokaty\"");
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}
