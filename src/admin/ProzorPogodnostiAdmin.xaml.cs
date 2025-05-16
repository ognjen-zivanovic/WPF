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
    public partial class ProzorPogodnostiAdmin : Window
    {
        public ProzorPogodnostiAdmin()
        {
            InitializeComponent();
            ShowAmenities();
        }

        private void ShowAmenities()
        {
            FontFamily unicodeFont = (FontFamily)Application.Current.Resources["UnicodeFont"];
            PanelZaPogodnosti.Children.Clear();
            // Assuming you have a method to get amenities from the database
            Amenity[] amenities = DatabaseManager.GetAllAmenities();
            foreach (var amenity in amenities)
            {
                StavkaMenjivePogodnostiAdmin amenityCard = new StavkaMenjivePogodnostiAdmin(amenity.Id, amenity.Name, amenity.Icon);

                amenityCard.DeleteButton.Click += (s, e) =>
                {
                    if (DeleteAmenityItem(amenity.Id))
                    {
                        PanelZaPogodnosti.Children.Remove(amenityCard);
                    }
                };

                PanelZaPogodnosti.Children.Add(amenityCard);
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

        private bool DeleteAmenityItem(int amenityId)
        {
            Room[] rooms = DatabaseManager.GetRoomsByAmenityId(amenityId);
            if (rooms.Length > 0)
            {
                MessageBox.Show("Cannot delete this amenity as it is associated with existing rooms.");
                return false;
            }
            DatabaseManager.DeleteAmenity(amenityId);
            return true;
        }
    }
}