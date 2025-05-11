using System;
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
    public partial class CheckoutWindow : Window
    {

        public Room Room { get; set; }
        public float TotalPrice { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public int NumberOfBabies { get; set; }
        public string AmenitiesText { get; set; }
        public string NumberOfGuestsText { get; set; }


        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string UserEmail { get; set; }
        public string UserPhoneNumber { get; set; }

        public CheckoutWindow()
        {
            InitializeComponent();
        }

        public CheckoutWindow(int roomId, DateTime checkInDate, DateTime checkOutDate, float totalPrice, int numberOfGuests, int numberOfBabies, string amenitiesText)
        {
            InitializeComponent();

            Room = DatabaseManager.GetRoomWithId(roomId);
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            TotalPrice = totalPrice;
            NumberOfGuests = numberOfGuests;
            NumberOfBabies = numberOfBabies;
            AmenitiesText = amenitiesText;

            if (NumberOfBabies > 0)
            {
                NumberOfGuestsText = $"{NumberOfGuests} + {NumberOfBabies} babies";
            }
            else
            {
                NumberOfGuestsText = $"{NumberOfGuests}";
            }

            for (int i = 0; i < NumberOfGuests; i++)
            {
                GuestNameCard guestNameCard = new GuestNameCard();
                GuestNamesStackPanel.Children.Add(guestNameCard);
            }

            this.DataContext = this;

        }

        public void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            // show all info 
            MessageBox.Show($"Room: {Room.Name}\n" +
                $"Check-in: {CheckInDate.ToShortDateString()}\n" +
                $"Check-out: {CheckOutDate.ToShortDateString()}\n" +
                $"Total Price: {TotalPrice} €\n" +
                $"Number of guests: {NumberOfGuestsText}\n" +
                $"Amenities: {AmenitiesText}");
            MessageBox.Show($"User Name: {UserName}\n" +
                $"User Surname: {UserSurname}\n" +
                $"User Email: {UserEmail}\n" +
                $"User Phone: {UserPhoneNumber}");
            foreach (GuestNameCard guestNameCard in GuestNamesStackPanel.Children)
            {
                MessageBox.Show($"Guest Name: {guestNameCard.GuestName}\n" +
                    $"Guest Surname: {guestNameCard.GuestSurname}");
            }
        }
    }
}
