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
    public partial class AdminAmenitiesWindow : Window
    {
        public AdminAmenitiesWindow()
        {
            InitializeComponent();
            ShowAmenities();
        }

        private void ShowAmenities()
        {
            FontFamily unicodeFont = (FontFamily)Application.Current.Resources["UnicodeFont"];
            AmenitiesStackPanel.Children.Clear();
            // Assuming you have a method to get amenities from the database
            Amenity[] amenities = DatabaseManager.GetAllAmenities();
            foreach (var amenity in amenities)
            {
                var amenityCard = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(10),
                    Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                    Width = 300,
                    Height = 100,
                };

                var iconTextBox = new TextBox
                {
                    Text = amenity.Icon,
                    FontSize = 20,
                    FontFamily = unicodeFont,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10)
                };

                var nameTextBox = new TextBox
                {
                    Text = amenity.Name,
                    FontSize = 20,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10)
                };

                iconTextBox.TextChanged += (s, e) =>
                {
                    amenity.Icon = ((TextBox)s).Text;
                    DatabaseManager.UpdateAmenity(amenity.Id, amenity.Name, amenity.Icon);
                };

                nameTextBox.TextChanged += (s, e) =>
                {
                    amenity.Name = ((TextBox)s).Text;
                    DatabaseManager.UpdateAmenity(amenity.Id, amenity.Name, amenity.Icon);
                };

                amenityCard.Children.Add(iconTextBox);
                amenityCard.Children.Add(nameTextBox);


                var deleteButton = new Button
                {
                    Content = "Delete",
                    Margin = new Thickness(10),
                    Width = 75,
                    Height = 30,
                    Background = new SolidColorBrush(Color.FromRgb(255, 0, 0)),
                    Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255))
                };

                deleteButton.Click += (s, e) =>
                {
                    Room[] rooms = DatabaseManager.GetRoomsByAmenityId(amenity.Id);
                    if (rooms.Length > 0)
                    {
                        MessageBox.Show("Cannot delete this amenity as it is associated with existing rooms.");
                        return;
                    }
                    DatabaseManager.DeleteAmenity(amenity.Id);
                    ShowAmenities();
                };

                amenityCard.Children.Add(deleteButton);

                AmenitiesStackPanel.Children.Add(amenityCard);
            }
        }


        private void AddAmenityButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the AddAmenityWindow when the button is clicked
            Amenity defaultAmenity = new Amenity
            {
                Name = "New Amenity",
                Icon = "",
            };
            DatabaseManager.InsertAmenity(defaultAmenity);
            ShowAmenities();
        }
    }
}