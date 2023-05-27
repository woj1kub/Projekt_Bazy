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
            bool VerLogin = !VerifyLogin(login);
            bool VerPass = VerifyHaslo(pass, apppass);
            if ( !(VerLogin && VerPass))
                return false;
            byte[] salt;
            string hashedPass = PasswordInterface.HashPasword(pass, out salt);
            
            using (var conn = new NpgsqlConnection(ConnString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand($"INSERT INTO \"Użytkownicy\" VALUES ('{login}' , '{hashedPass}' , '{salt}')");
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

            return (SamePass && SafePass);
        }

        static public bool VerifyLogin(string login) 
        {
            using (var conn = new NpgsqlConnection(ConnString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand($"SELECT* FROM \"Użytkownicy\"");
                cmd.Connection = conn;
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<String> listUsers = new List<string>();
                while (reader.Read())
                {
                    listUsers.Add(reader.GetString(0));
                }

                if (listUsers.Contains(login))
                {
                    conn.Close();
                    return true;
                }
                else
                {
                    conn.Close();
                    return false;
                }

            }
        }
        static public void AllUsersShow()
        {
            using (var conn = new NpgsqlConnection(ConnString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"Użytkownicy\"");
                cmd.Connection = conn;
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<String> listUsers = new List<string>();
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
