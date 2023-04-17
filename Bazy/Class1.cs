using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bazy
{
    public class Class1
    {
        static public void TestWyswietl(MainWindow a)
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
            using (var conn = new NpgsqlConnection(connStringBuilder.ConnectionString))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT* FROM \"Użytkownicy\"");
                cmd.Connection = conn;
                NpgsqlDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                    MessageBox.Show(reader["Login"].ToString() + reader["Hasło"].ToString() + reader["Sól"].ToString());
                }
                conn.Close();
            }
        }
        static public void TestInsert(MainWindow a)
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
            using (var conn = new NpgsqlConnection(connStringBuilder.ConnectionString))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO \"Użytkownicy\" VALUES ('LoginTest2', 'HasłoTest2', '105')");
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        static public void TestDelete(MainWindow a)
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
            using (var conn = new NpgsqlConnection(connStringBuilder.ConnectionString))
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
