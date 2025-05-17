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
    public partial class GlavniProzorAplikacije : Window
    {
        public GlavniProzorAplikacije()
        {
            InitializeComponent();
        }

        private void PrikaziAdminProzor_Click(object sender, RoutedEventArgs e)
        {
            GlavniProzorAdmin glavniProzorAdmin = new GlavniProzorAdmin();
            glavniProzorAdmin.Show();
            this.Close();
        }

        private void PrikaziKorisnickiProzor_Click(object sender, RoutedEventArgs e)
        {
            GlavniProzorKorisnik glavniProzorKorisnik = new GlavniProzorKorisnik();
            glavniProzorKorisnik.Show();
            this.Close();
        }
    }
}
