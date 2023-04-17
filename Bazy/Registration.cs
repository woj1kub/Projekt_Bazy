using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        static public void RegisterUser(string login,string pass)
        {
            if(VerifyLogin(login)) return;
            string hashedPass = PasswordInterface.HashPasword(pass, out var salt);
            using (var conn = new NpgsqlConnection(ConnString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand($"INSERT INTO \"Użytkownicy\" VALUES ('{login}' , '{hashedPass}' , '{Convert.ToHexString(salt)}')");
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                conn.Close();
            }

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
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT* FROM \"Użytkownicy\"");
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
