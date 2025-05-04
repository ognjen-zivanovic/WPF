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

    public partial class RoomCard : UserControl
    {
        public int RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public float Price { get; set; }

        public RoomCard()
        {
            InitializeComponent();
        }

        public void SetRoomData(Room room, Amenity[] amenities, ImageSource imageSource, float price)
        {
            RoomId = room.Id;
            Price = price;

            RoomImage.Source = imageSource;
            CapacityText.Text = $"Capacity: {room.Capacity}";
            PriceText.Text = $"{price}";
            RoomNameText.Text = room.Name;
            DescriptionText.Text = room.Description;

            AmenityPanel.Children.Clear();
            foreach (var amenity in amenities)
            {
                AmenityPanel.Children.Add(new Image
                {
                    Source = DatabaseManager.SourceFromByteArray(amenity.Icon),
                    Width = 50,
                    Height = 50,
                    Margin = new Thickness(5),
                    ToolTip = amenity.Name
                });
            }
        }

        private void BookRoom_Click(object sender, RoutedEventArgs e)
        {
            var checkoutWindow = new CheckoutWindow(RoomId, CheckIn, CheckOut, Price);
            checkoutWindow.Show();
        }
    }
}