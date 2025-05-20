using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HotelRezervacije
{

    public partial class KarticaSobeKorisnik : UserControl
    {
        public int SobaId { get; set; }
        public DateTime DatumDolaska { get; set; }
        public DateTime DatumOdlaska { get; set; }
        public decimal UkupnaCena { get; set; }
        public Pogodnost[] Pogodnosti { get; set; }
        public int BrojGostiju { get; set; }
        public int BrojBeba { get; set; }

        public KarticaSobeKorisnik()
        {
            InitializeComponent();
        }

        public void PostaviPodatke(Soba soba, Pogodnost[] pogodnosti, ImageSource izvorSlike, decimal cena, string datumDolaska, string datumOdlaska, int brojGostiju, int brojBeba)
        {
            SobaId = soba.Id;
            UkupnaCena = cena;
            Pogodnosti = pogodnosti;
            DatumDolaska = DateTime.Parse(datumDolaska);
            DatumOdlaska = DateTime.Parse(datumOdlaska);
            BrojGostiju = brojGostiju;
            BrojBeba = brojBeba;

            SlikaSobe.Source = izvorSlike;
            KapacitetTekst.Text = $"Kapacitet: {soba.Kapacitet}";
            CenaTekst.Text = $"{UkupnaCena}€";
            NazivSobeTekst.Text = soba.Ime;
            OpisTekst.Text = soba.Opis;

            PanelPogodnosti.Children.Clear();
            foreach (var pogodnost in pogodnosti)
            {
                PanelPogodnosti.Children.Add(new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Children =
                    {
                        new TextBlock
                        {
                            FontFamily = MenadzerResursa.UnikodFont,
                            FontSize = 18,
                            Text = pogodnost.Ikonica,
                            VerticalAlignment = VerticalAlignment.Center,
                        },
                        new TextBlock
                        {
                            FontSize = 18,
                            Text = pogodnost.Ime,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(5, 0, 10, 5),
                        }
                    }
                });
            }
        }

        private void RezervisiSobu_Click(object sender, RoutedEventArgs e)
        {
            var prozorRezervacije = new ProzorRezervacijeKorisnik(SobaId, DatumDolaska, DatumOdlaska, UkupnaCena, BrojGostiju, BrojBeba, string.Join(", ", Pogodnosti.Select(a => a.Ime)));
            prozorRezervacije.Show();
        }
    }
}