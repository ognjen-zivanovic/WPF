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
    public partial class AdminMainWindow : Window
    {
        public AdminMainWindow()
        {
            InitializeComponent();
        }

        private void EditRooms_Click(object sender, RoutedEventArgs e)
        {
            AdminRoomsWindow adminRooms = new AdminRoomsWindow();
            adminRooms.Show();
            this.Close();
        }

        private void ViewReservations_Click(object sender, RoutedEventArgs e)
        {
            AdminReservationsWindow adminReservations = new AdminReservationsWindow();
            adminReservations.Show();
            this.Close();
        }
    }
}