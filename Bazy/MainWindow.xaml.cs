using System;
using System.Collections.Generic;
using System.Linq;
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
            test_hasla();
            InitializeComponent();
        }
        void test_hasla()
        {
            string test_psd = "ZAQ!1qaz";
            byte[] user_salt;
            string hash = PasswordInterface.HashPasword(test_psd, out user_salt);

            MessageBox.Show(PasswordInterface.VerifyPassword(test_psd, hash, user_salt).ToString());
        }
    }
}
