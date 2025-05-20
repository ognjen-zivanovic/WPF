using System.Windows.Controls;

namespace HotelRezervacije
{
    public partial class KarticaRezervacijeAdmin : UserControl
    {
        public Soba Soba { get; set; }
        public Rezervacija Rezervacija { get; set; }
        public Korisnik Korisnik { get; set; }
        public Gost[] Gosti { get; set; }
        public string PogodnostiTekst { get; set; }
        public KarticaRezervacijeAdmin()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public KarticaRezervacijeAdmin(Soba soba, Rezervacija rezervacija, Korisnik korisnik, Gost[] gosti)
        {
            InitializeComponent();

            Soba = soba;
            Rezervacija = rezervacija;
            Korisnik = korisnik;
            Gosti = gosti;

            Pogodnost[] pogodnosti = MenadzerBazePodataka.UcitajPogodnostiZaSobu(soba.Id);
            PogodnostiTekst = string.Join(", ", pogodnosti.Select(a => a.Ime));

            NazivSobeTekst.Text = soba.Ime;
            IDTekst.Text = "ID Sobe: " + soba.Id + " | ID Rezervacije: " + rezervacija.Id;
            DatumTekst.Text = "Datum dolaska: " + rezervacija.DatumDolaska.ToString("dd/MM/yyyy") + " | Datum odlaska: " + rezervacija.DatumOdlaska.ToString("dd/MM/yyyy");
            CenaTekst.Text = "Ukupna cena:" + rezervacija.UkupnaCena.ToString("C");
            ImeKorisnikaTekst.Text = "Kupac: " + korisnik.Ime + " " + korisnik.Prezime;
            KontaktKorisnikaTekst.Text = "Kontakt: " + korisnik.Email + " | " + korisnik.Telefon;
            BrojGostijuTekst.Text = "Broj gostiju: " + rezervacija.BrojGostiju;

            this.DataContext = this;
        }
    }
}