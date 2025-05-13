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
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace HotelRezervacije
{
    public partial class CheckoutWindow : Window
    {
        public Room Room { get; set; }
        public decimal TotalPriceNumber { get; set; }
        public int NumberOfPeople { get; set; }

        public CheckoutWindow()
        {
            InitializeComponent();
        }

        public CheckoutWindow(int roomId, DateTime checkInDate, DateTime checkOutDate, decimal totalPrice, int numberOfGuests, int numberOfBabies, string amenitiesText)
        {
            InitializeComponent();

            Room = DatabaseManager.GetRoomWithId(roomId);

            RoomName.Text = Room.Name;
            RoomDescription.Text = Room.Description;
            RoomCapacity.Text = "Capacity: " + Room.Capacity.ToString();

            AmenitiesText.Text = amenitiesText;

            TotalPrice.Text = totalPrice.ToString() + " €";
            CheckInDate.Text = checkInDate.ToShortDateString();
            CheckOutDate.Text = checkOutDate.ToShortDateString();

            NumberOfPeople = numberOfGuests;
            TotalPriceNumber = totalPrice;


            if (numberOfBabies > 0)
            {
                NumberOfGuests.Text = $"{numberOfGuests} + {numberOfBabies} babies";
            }
            else
            {
                NumberOfGuests.Text = $"{numberOfGuests}";
            }

            for (int i = 0; i < numberOfGuests; i++)
            {
                GuestNameCard guestNameCard = new GuestNameCard();
                GuestNamesStackPanel.Children.Add(guestNameCard);
            }

            this.DataContext = this;

        }

        public bool IsEmailValid(string emailaddress)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(emailaddress);
            return match.Success;
        }

        public bool IsPhoneNumberValid(string phoneNumber)
        {
            Regex regex = new Regex(@"^\+?[0-9]{10,15}$");
            Match match = regex.Match(phoneNumber);
            return match.Success;
        }

        public void SetErrorText(string text)
        {
            ErrorText.Text = text;
            ErrorText.Visibility = text != "" ? Visibility.Visible : Visibility.Collapsed;
        }


        public void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(UserName.Text) || string.IsNullOrWhiteSpace(UserSurname.Text) ||
                string.IsNullOrWhiteSpace(UserEmail.Text) || string.IsNullOrWhiteSpace(UserPhoneNumber.Text))
            {

                SetErrorText("Please fill in all the fields.");
                return;
            }
            else if (!IsEmailValid(UserEmail.Text))
            {

                SetErrorText("Please enter a valid email address.");
                return;
            }
            else if (!IsPhoneNumberValid(UserPhoneNumber.Text))
            {
                SetErrorText("Please enter a valid phone number. Phone number format is + (country code) (number). For example +381641234567");
                return;
            }
            else
            {
                SetErrorText("");
            }

            foreach (GuestNameCard guestNameCard in GuestNamesStackPanel.Children)
            {
                if (string.IsNullOrWhiteSpace(guestNameCard.GuestName) || string.IsNullOrWhiteSpace(guestNameCard.GuestSurname))
                {
                    SetErrorText("Please fill in all the fields.");
                    return;
                }
                else
                {
                    SetErrorText("");
                }
            }

            User newUser = new User
            {
                Name = UserName.Text,
                Surname = UserSurname.Text,
                Email = UserEmail.Text,
                Phone = UserPhoneNumber.Text
            };

            int userId = DatabaseManager.InsertUser(newUser);

            Reservation newReservation = new Reservation
            {
                RoomId = Room.Id,
                UserId = userId,
                CheckIn = DateTime.Parse(CheckInDate.Text),
                CheckOut = DateTime.Parse(CheckOutDate.Text),
                TotalPrice = TotalPriceNumber,
                NumberOfGuests = NumberOfPeople,
            };

            int reservationId = DatabaseManager.InsertReservation(newReservation);

            foreach (GuestNameCard guestNameCard in GuestNamesStackPanel.Children)
            {
                Guest newGuest = new Guest
                {
                    Name = guestNameCard.GuestName,
                    Surname = guestNameCard.GuestSurname
                };

                int guestId = DatabaseManager.InsertGuest(newGuest);

                DatabaseManager.InsertGuestReservation(reservationId, guestId);
            }
        }
    }
}
