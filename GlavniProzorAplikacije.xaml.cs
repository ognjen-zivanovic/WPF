using System.Windows;

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
