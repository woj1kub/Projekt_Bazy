﻿using Npgsql;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Bazy
{
    /// <summary>
    /// Logika interakcji dla klasy Ustawienia.xaml
    /// </summary>
    public partial class UstawieniaPanel : UserControl
    {
        readonly String ActiveUser;
        public UstawieniaPanel(string activeUser)
        {
            InitializeComponent();
            ActiveUser = activeUser;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtNoweHaslo.Password.Equals(txtPotwierdzNoweHaslo.Password) && PasswordInterface.VerifyIfpasswordIsSafe(txtNoweHaslo.Password))
            {
                string pass = txtNoweHaslo.Password;
                byte[] salt;
                string hashedPass = PasswordInterface.HashPasword(pass, out salt);

                using (var conn = new NpgsqlConnection(Registration.ConnString()))
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand($"UPDATE \"Użytkownicy\" SET \"Hasło\" = @hashPass , \"Sól\"=@salt WHERE \"Login\"=@login");
                    cmd.Parameters.AddWithValue("@login", ActiveUser);
                    cmd.Parameters.AddWithValue("@hashPass", hashedPass);
                    cmd.Parameters.AddWithValue("@salt", salt);
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            txtNoweHaslo.Clear();
            txtPotwierdzNoweHaslo.Clear();
        }
    }
}
