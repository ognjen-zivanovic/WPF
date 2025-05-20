using System.Windows;

namespace HotelRezervacije
{
    public partial class ProzorRezervacijaAdmin : Window
    {
        public ProzorRezervacijaAdmin()
        {
            InitializeComponent();
        }

        public void PretragaDugme_Click(object sender, RoutedEventArgs e)
        {
            string pretragaTekst = PretragaTextbox.Text;

            KarticaRezervacije[] karticeRezervacije = MenadzerBazePodataka.UcitajPretrazeneRezervacije(pretragaTekst);

            PanelRezervacija.Children.Clear();

            foreach (var karticaRezervacije in karticeRezervacije)
            {
                Gost[] gosti = MenadzerBazePodataka.UcitajGostePoIdRezervacije(karticaRezervacije.Rezervacija.Id);

                KarticaRezervacijeAdmin kartica = new KarticaRezervacijeAdmin(karticaRezervacije.Soba, karticaRezervacije.Rezervacija, karticaRezervacije.Korisnik, gosti);

                PanelRezervacija.Children.Add(kartica);
            }
        }
    }
    public class KarticaRezervacije
    {
        public Rezervacija Rezervacija { get; set; }
        public Soba Soba { get; set; }
        public Korisnik Korisnik { get; set; }
    }
}