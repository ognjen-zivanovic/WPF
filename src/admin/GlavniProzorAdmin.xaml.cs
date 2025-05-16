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
            this.Close();
        }

        private void IzmenaSoba_Click(object sender, RoutedEventArgs e)
        {
            ProzorSobaAdmin adminRooms = new ProzorSobaAdmin();
            adminRooms.Show();
            this.Close();
        }

        private void IzmenaPogodnosti_Click(object sender, RoutedEventArgs e)
        {
            ProzorPogodnostiAdmin adminAmenities = new ProzorPogodnostiAdmin();
            adminAmenities.Show();
            this.Close();
        }
    }
}