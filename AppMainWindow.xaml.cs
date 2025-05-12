using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data.SQLite;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;

namespace WpfApp1
{
    public partial class AppMainWindow : Window
    {
        public AppMainWindow()
        {
            InitializeComponent();
        }

        private void ShowAdminWindowButton_Click(object sender, RoutedEventArgs e)
        {
            AdminMainWindow adminMainWindow = new AdminMainWindow();
            adminMainWindow.Show();
            this.Close();
        }

        private void ShowUserWindowButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
