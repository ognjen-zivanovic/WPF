using System;
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

            Pogodnost[] pogodnosti = DatabaseManager.UcitajPogodnostiZaSobu(soba.Id);
            PogodnostiTekst = string.Join(", ", pogodnosti.Select(a => a.Ime));

            NazivSobeTekst.Text = soba.Ime;
            IDTekst.Text = "Room ID: " + soba.Id + " | Reservation ID: " + rezervacija.Id;
            DatumTekst.Text = "Check-in: " + rezervacija.CheckIn.ToString("dd/MM/yyyy") + " | Check-out: " + rezervacija.CheckOut.ToString("dd/MM/yyyy");
            CenaTekst.Text = "Total Price: " + rezervacija.UkupnaCena.ToString("C");
            ImeKorisnikaTekst.Text = "User: " + korisnik.Ime + " " + korisnik.Prezime;
            KontaktKorisnikaTekst.Text = "Contact: " + korisnik.Email + " | " + korisnik.Telefon;
            BrojGostijuTekst.Text = "Guests: " + rezervacija.BrojGostiju;

            this.DataContext = this;
        }
    }
}