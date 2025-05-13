using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace HotelRezervacije
{
    public partial class AdminAmenityItem : UserControl
    {
        public event RoutedEventHandler Deleted;

        public int AmenityId { get; set; }

        public AdminAmenityItem(int id, string name, string icon)
        {
            InitializeComponent();
            RootGrid.MouseEnter += (s, e) => DeleteButton.Visibility = Visibility.Visible;
            RootGrid.MouseLeave += (s, e) => DeleteButton.Visibility = Visibility.Hidden;

            AmenityName.Text = name;
            AmenityIcon.Text = icon;
            AmenityId = id;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Deleted?.Invoke(this, new RoutedEventArgs());
        }
    }
}
