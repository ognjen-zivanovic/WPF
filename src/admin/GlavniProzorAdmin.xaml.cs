using System.Windows;

namespace HotelRezervacije
{
    public partial class GlavniProzorAdmin : Window
    {
        public GlavniProzorAdmin()
        {
            InitializeComponent();
        }

        private void PregledRezervacija_Click(object sender, RoutedEventArgs e)
        {
            ProzorRezervacijaAdmin adminReservations = new ProzorRezervacijaAdmin();
            adminReservations.Show();
        }

        private void IzmenaSoba_Click(object sender, RoutedEventArgs e)
        {
            ProzorSobaAdmin adminRooms = new ProzorSobaAdmin();
            adminRooms.Show();
        }

        private void IzmenaPogodnosti_Click(object sender, RoutedEventArgs e)
        {
            ProzorPogodnostiAdmin adminAmenities = new ProzorPogodnostiAdmin();
            adminAmenities.Show();
        }
    }
}