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
    public partial class ProzorSobaAdmin : Window
    {
        public ProzorSobaAdmin()
        {
            InitializeComponent();
            ShowRooms();
        }

        private void ShowRooms()
        {
            AdminPanelZaSobe.Children.Clear();
            // Assuming you have a method to get rooms from the database
            Room[] rooms = DatabaseManager.GetAllRooms();
            foreach (var room in rooms)
            {
                var image = DatabaseManager.LoadImageFromDatabase(room.Id);
                var price = room.PricePerNight;

                KarticaSobeAdmin roomCard = new KarticaSobeAdmin();
                Amenity[] amenities = DatabaseManager.GetAmenitiesForRoom(room.Id);
                roomCard.SetRoomData(room, amenities, image);
                roomCard.DeleteButton.Click += (s, e) =>
                {
                    if (DeleteRoom(room.Id))
                    {
                        AdminPanelZaSobe.Children.Remove(roomCard);
                    }
                };
                AdminPanelZaSobe.Children.Add(roomCard);
            }
        }

        private void AddRoomButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the AddRoomWindow when the button is clicked
            Room defaultRoom = new Room
            {
                Name = "New Room",
                Capacity = 2,
                PricePerNight = 100.00m,
                Description = "Description of the new room."
            };
            int insertedRoomId = DatabaseManager.InsertRoom(defaultRoom);
            DatabaseManager.InsertImage(insertedRoomId, null);
            ShowRooms();
        }


        private bool DeleteRoom(int RoomId)
        {
            Reservation[] reservations = DatabaseManager.GetReservationsForRoom(RoomId);
            if (reservations.Length > 0)
            {
                MessageBox.Show("Cannot delete room with existing reservations.");
                return false;
            }

            DatabaseManager.DeleteRoom(RoomId);
            DatabaseManager.DeleteImageWithRoomId(RoomId);
            DatabaseManager.DeleteAllAmenitiesFromRoom(RoomId);
            return true;
        }
    }
}