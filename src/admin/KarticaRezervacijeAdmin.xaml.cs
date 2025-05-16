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

namespace HotelRezervacije
{
    public partial class KarticaRezervacijeAdmin : UserControl
    {
        public Room Room { get; set; }
        public Reservation Reservation { get; set; }
        public User User { get; set; }
        public Guest[] Guests { get; set; }
        public string AmenitiesText { get; set; }
        public KarticaRezervacijeAdmin()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public KarticaRezervacijeAdmin(Room room, Reservation reservation, User user, Guest[] guests)
        {
            InitializeComponent();

            Room = room;
            Reservation = reservation;
            User = user;
            Guests = guests;

            Amenity[] amenities = DatabaseManager.GetAmenitiesForRoom(room.Id);
            AmenitiesText = string.Join(", ", amenities.Select(a => a.Name));

            RoomNameText.Text = room.Name;
            IDText.Text = "Room ID: " + room.Id + " | Reservation ID: " + reservation.Id;
            DateText.Text = "Check-in: " + reservation.CheckIn.ToString("dd/MM/yyyy") + " | Check-out: " + reservation.CheckOut.ToString("dd/MM/yyyy");
            PriceText.Text = "Total Price: " + reservation.TotalPrice.ToString("C");
            UserNameText.Text = "User: " + user.Name + " " + user.Surname;
            UserContactText.Text = "Contact: " + user.Email + " | " + user.Phone;
            GuestNumberText.Text = "Guests: " + reservation.NumberOfGuests;

            this.DataContext = this;
        }
    }
}