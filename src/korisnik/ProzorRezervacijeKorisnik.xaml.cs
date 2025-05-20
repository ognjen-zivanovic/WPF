using System.Windows;
using System.Text.RegularExpressions;

namespace HotelRezervacije
{
    public partial class ProzorRezervacijeKorisnik : Window
    {
        public Soba Soba { get; set; }
        public decimal UkupnaCenaBroj { get; set; }
        public int BrojLjudi { get; set; }

        public ProzorRezervacijeKorisnik()
        {
            InitializeComponent();
        }

        public ProzorRezervacijeKorisnik(int sobaId, DateTime datumDolaska, DateTime datumOdlaska, decimal ukupnaCena, int brojGostiju, int brojBeba, string pogodnostiText)
        {
            InitializeComponent();

            Soba = MenadzerBazePodataka.UcitajSobuSaId(sobaId);

            ImeSobe.Text = Soba.Ime;
            OpisSobe.Text = Soba.Opis;
            KapacitetSobe.Text = "Kapacitet: " + Soba.Kapacitet.ToString();

            PogodnostiTekst.Text = pogodnostiText;

            UkupnaCena.Text = ukupnaCena.ToString() + " €";
            DatumDolaska.Text = datumDolaska.ToShortDateString();
            DatumOdlaska.Text = datumOdlaska.ToShortDateString();

            BrojLjudi = brojGostiju;
            UkupnaCenaBroj = ukupnaCena;

            if (brojBeba > 0)
            {
                BrojGostiju.Text = $"{brojGostiju} + {brojBeba} beba";
            }
            else
            {
                BrojGostiju.Text = $"{brojGostiju}";
            }

            for (int i = 0; i < brojGostiju; i++)
            {
                KarticaImenaKorisnik karticaImenaKorisnik = new KarticaImenaKorisnik();
                PanelImenaGostiju.Children.Add(karticaImenaKorisnik);
            }

            this.DataContext = this;

        }

        public bool EmailValidnost(string emailAdresa)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(emailAdresa);
            return match.Success;
        }

        public bool BrojTelefonaValidnost(string brojTelefona)
        {
            Regex regex = new Regex(@"^\+?[0-9]{10,15}$");
            Match match = regex.Match(brojTelefona);
            return match.Success;
        }

        public void PrikaziErrorTekst(string tekst)
        {
            ErrorTekst.Text = tekst;
            ErrorTekst.Visibility = tekst != "" ? Visibility.Visible : Visibility.Collapsed;
        }


        public void RezervacijaDugme_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(ImeKorisnika.Text) || string.IsNullOrWhiteSpace(PrezimeKorisnika.Text) ||
                string.IsNullOrWhiteSpace(EmailKorisnika.Text) || string.IsNullOrWhiteSpace(BrojTelefonaKorisnika.Text))
            {
                PrikaziErrorTekst("Molimo popunite sva polja.");
                return;
            }
            else if (!EmailValidnost(EmailKorisnika.Text))
            {
                PrikaziErrorTekst("Molimo unesete ispravnu email adresu.");
                return;
            }
            else if (!BrojTelefonaValidnost(BrojTelefonaKorisnika.Text))
            {
                PrikaziErrorTekst("Molimo unesete ispravni broj telefona. Format telefona je +(dr.)(broj). Na primer +381641234567");
                return;
            }
            else
            {
                PrikaziErrorTekst("");
            }

            foreach (KarticaImenaKorisnik karticaImenaKorisnik in PanelImenaGostiju.Children)
            {
                if (string.IsNullOrWhiteSpace(karticaImenaKorisnik.ImeGosta) || string.IsNullOrWhiteSpace(karticaImenaKorisnik.PrezimeGosta))
                {
                    PrikaziErrorTekst("Molimo popunite sva polja.");
                    return;
                }
                else
                {
                    PrikaziErrorTekst("");
                }
            }

            Korisnik noviKorisnik = new Korisnik
            {
                Ime = ImeKorisnika.Text,
                Prezime = PrezimeKorisnika.Text,
                Email = EmailKorisnika.Text,
                Telefon = BrojTelefonaKorisnika.Text
            };

            int korisnikId = MenadzerBazePodataka.DodajKorisnika(noviKorisnik);

            Rezervacija novaRezervacija = new Rezervacija
            {
                SobaId = Soba.Id,
                KorisnikId = korisnikId,
                DatumDolaska = DateTime.Parse(DatumDolaska.Text),
                DatumOdlaska = DateTime.Parse(DatumOdlaska.Text),
                UkupnaCena = UkupnaCenaBroj,
                BrojGostiju = BrojLjudi,
            };

            int rezervacijaId = MenadzerBazePodataka.DodajRezervaciju(novaRezervacija);

            foreach (KarticaImenaKorisnik karticaImenaKorisnik in PanelImenaGostiju.Children)
            {
                Gost noviGost = new Gost
                {
                    Ime = karticaImenaKorisnik.ImeGosta,
                    Prezime = karticaImenaKorisnik.PrezimeGosta
                };

                int gostId = MenadzerBazePodataka.DodajGosta(noviGost);

                MenadzerBazePodataka.DodajGostRezervaciju(rezervacijaId, gostId);

            }
            PrikaziProzorUspesneRezervacije(rezervacijaId);
        }

        private void PrikaziProzorUspesneRezervacije(int rezervacijaId)
        {
            ProzorUspesneRezervacije prozorUspesneRezervacije = new ProzorUspesneRezervacije(rezervacijaId);
            prozorUspesneRezervacije.Show();
            this.Close();
        }
    }
}
