using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace HotelRezervacije
{
    public partial class StavkaPogodnostiAdmin : UserControl
    {
        public event RoutedEventHandler Obrisano;

        public int PogodnostId { get; set; }

        public StavkaPogodnostiAdmin(int id, string ime, string ikonica)
        {
            InitializeComponent();
            GlavniGrid.MouseEnter += (s, e) => ObrisiDugme.Visibility = Visibility.Visible;
            GlavniGrid.MouseLeave += (s, e) => ObrisiDugme.Visibility = Visibility.Hidden;

            PogodnostIme.Text = ime;
            PogodnostIkonica.Text = ikonica;
            PogodnostId = id;
        }

        private void ObrisiDugme_Click(object sender, RoutedEventArgs e)
        {
            Obrisano?.Invoke(this, new RoutedEventArgs());
        }
    }
}
