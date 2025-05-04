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
    public partial class AdminRoomsWindow : Window
    {
        public AdminRoomsWindow()
        {
            InitializeComponent();
            ShowRooms();
        }

        private void ShowRooms()
        {
            // Assuming you have a method to get rooms from the database
            Room[] rooms = DatabaseManager.GetAllRooms();
            foreach (var room in rooms)
            {
                var image = DatabaseManager.LoadImageFromDatabase(room.Id);
                var price = room.PricePerNight;

                AdminRoomCard roomCard = new AdminRoomCard();
                Amenity[] amenities = DatabaseManager.GetAmenitiesForRoom(room.Id);
                roomCard.SetRoomData(room, amenities, image);
                AdminRoomsStackPanel.Children.Add(roomCard);
            }
        }
    }
}