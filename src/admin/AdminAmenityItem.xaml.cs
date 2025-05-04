using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace WpfApp1
{
    public partial class AdminAmenityItem : UserControl
    {
        public event RoutedEventHandler Deleted;

        public AdminAmenityItem()
        {
            InitializeComponent();
            RootGrid.MouseEnter += (s, e) => DeleteButton.Visibility = Visibility.Visible;
            RootGrid.MouseLeave += (s, e) => DeleteButton.Visibility = Visibility.Hidden;
        }

        public int AmenityId { get; set; }
        public string AmenityName { get; set; }

        public void SetImageSource(ImageSource imageSource)
        {
            IconImage.Source = imageSource;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Deleted?.Invoke(this, new RoutedEventArgs());
        }
    }
}
