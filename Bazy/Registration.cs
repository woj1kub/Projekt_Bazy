using Npgsql;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Bazy
{
    static internal class Registration
    {
        static public string ConnString()
        {
            var connStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = "test-bazy-danych-6865.7tc.cockroachlabs.cloud",
                Port = 26257,
                SslMode = SslMode.VerifyFull,
                Username = "wojkub",
                Password = "3GRP9cOzNFBbrR0Ea_PJ5w",
                Database = "defaultdb"
            };
            return connStringBuilder.ConnectionString;
        }

        static public bool RegisterUser(string login,string pass, string apppass)
        {
            bool VerLogin = VerifyLogin(login);
            bool VerPass = VerifyHaslo(pass, apppass);
            if ( !(VerLogin && VerPass))
                return false;
            byte[] salt;
            string hashedPass = PasswordInterface.HashPasword(pass, out salt);
            
            using (var conn = new NpgsqlConnection(ConnString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand($"INSERT INTO \"Użytkownicy\" VALUES (@login , @hashPass , @salt )");
                cmd.Parameters.AddWithValue("@login", login);
                cmd.Parameters.AddWithValue("@hashPass", hashedPass);
                cmd.Parameters.AddWithValue("@salt", salt);
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            return true;
        }

        static public bool VerifyHaslo(string pass, string apppass)
        {
            bool SamePass = pass == apppass;
            bool SafePass = PasswordInterface.VerifyIfpasswordIsSafe(pass);

            return SamePass && SafePass;
        }

        static public bool VerifyLogin(string login) 
        {
            using (var conn = new NpgsqlConnection(ConnString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand($"SELECT COUNT(*) FROM \"Użytkownicy\" WHERE \"Login\" = @login");
                cmd.Parameters.AddWithValue("@login", login);
                cmd.Connection = conn;
                NpgsqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                var NumberOfLogin = reader.GetInt64(0);
                if (NumberOfLogin==0)
                    return true;
                else
                    return false;
            }

        }



        //Testowanie
        static public void AllUsersShow()
        {
            using (var conn = new NpgsqlConnection(ConnString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"Użytkownicy\"");
                cmd.Connection = conn;
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<String> listUsers = new();
                while (reader.Read()) 
                {
                    listUsers.Add(reader.GetString(0));
                }
                string s=new string("");
                foreach (String x in listUsers) 
                {
                    s += x + "\n";
                }
                MessageBox.Show(s);
                conn.Close();
            }
        }
        static public void DeleteAllUsers()//temporary method
        {
            using (var conn = new NpgsqlConnection(ConnString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM \"Użytkownicy\"");
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}
