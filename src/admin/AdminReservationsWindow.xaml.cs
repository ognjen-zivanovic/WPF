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
    public partial class AdminReservationsWindow : Window
    {
        public AdminReservationsWindow()
        {
            InitializeComponent();
        }

        public void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchTextBox.Text;


            // MessageBox.Show($"Searching for: {searchText}");
            ReservationCard[] reservationCards = DatabaseManager.GetFilteredReservations(searchText);


            ReservationsStackPanel.Children.Clear();

            foreach (var reservationCard in reservationCards)
            {
                Guest[] guests = DatabaseManager.GetGuestsByReservationId(reservationCard.Reservation.Id);
                // Create a new instance of AdminReservationCard for each reservation
                AdminReservationCard card = new AdminReservationCard(reservationCard.Room, reservationCard.Reservation, reservationCard.User, guests);
                // Add the card to the UI (assuming you have a container like a StackPanel or Grid)
                ReservationsStackPanel.Children.Add(card);
            }
        }
    }
    public class ReservationCard
    {
        public Reservation Reservation { get; set; }
        public Room Room { get; set; }
        public User User { get; set; }
    }
}