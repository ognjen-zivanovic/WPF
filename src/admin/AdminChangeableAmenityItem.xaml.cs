using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace HotelRezervacije
{
    public partial class AdminChangeableAmenityItem : UserControl
    {
        Amenity amenity;
        public AdminChangeableAmenityItem(int id, string name, string icon)
        {
            InitializeComponent();
            amenity = new Amenity
            {
                Id = id,
                Name = name,
                Icon = icon
            };

            IconTextBox.Text = icon;
            NameTextBox.Text = name;
        }
        private void IconTextBox_TextChanged(object s, TextChangedEventArgs e)
        {
            amenity.Icon = ((TextBox)s).Text;
            DatabaseManager.UpdateAmenity(amenity.Id, amenity.Name, amenity.Icon);
        }
        private void NameTextBox_TextChanged(object s, TextChangedEventArgs e)
        {
            amenity.Name = ((TextBox)s).Text;
            DatabaseManager.UpdateAmenity(amenity.Id, amenity.Name, amenity.Icon);
        }
    }
}
