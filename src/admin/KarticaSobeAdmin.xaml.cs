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
using Microsoft.Win32;

namespace HotelRezervacije
{

    public partial class KarticaSobeAdmin : UserControl
    {
        public int SobaId { get; set; }

        public KarticaSobeAdmin()
        {
            InitializeComponent();
        }

        public void PostaviPodatkeSobe(Soba soba, Pogodnost[] pogodnosti, ImageSource izvorSlike)
        {
            SobaId = soba.Id;

            if (izvorSlike != null)
            {
                SlikaSobe.Source = izvorSlike;
            }
            else
            {
                SlikaSobe.Source = MenadzerResursa.IzvorOdImenaFajla("slike/nema_slike.png");
            }

            SlikaSobe.MouseEnter += (s, e) =>
            {
                ZameniSliku.Visibility = Visibility.Visible;
            };
            SlikaSobe.MouseLeave += (s, e) =>
            {
                ZameniSliku.Visibility = Visibility.Hidden;
            };

            ZameniSliku.MouseEnter += (s, e) =>
            {
                ZameniSliku.Visibility = Visibility.Visible;
            };


            ZameniSliku.Source = MenadzerResursa.IzvorOdImenaFajla("slike/ikonica_promeni_sliku.png");

            KapacitetTekst.Text = $"{soba.Kapacitet}";
            CenaTekst.Text = $"{soba.CenaPoNoci}";
            NazivSobeTekst.Text = soba.Ime;
            OpisTekst.Text = soba.Opis;

            KapacitetTekst.BorderBrush = Brushes.Gray;
            CenaTekst.BorderBrush = Brushes.Gray;
            NazivSobeTekst.BorderBrush = Brushes.Gray;
            OpisTekst.BorderBrush = Brushes.Gray;

            KapacitetTekst.TextChanged += (s, e) => KapacitetTekst.BorderBrush = Brushes.Black;
            CenaTekst.TextChanged += (s, e) => CenaTekst.BorderBrush = Brushes.Black;
            NazivSobeTekst.TextChanged += (s, e) => NazivSobeTekst.BorderBrush = Brushes.Black;
            OpisTekst.TextChanged += (s, e) => OpisTekst.BorderBrush = Brushes.Black;


            PanelPogodnosti.Children.Clear();
            foreach (var amenity in pogodnosti)
            {
                var stavka = new StavkaPogodnostiAdmin(amenity.Id, amenity.Ime, amenity.Ikonica);
                stavka.Obrisano += (s, e) =>
                {
                    MenadzerBazePodataka.ObrisiPogodnostIzSobe(soba.Id, amenity.Id);
                    PanelPogodnosti.Children.Remove(stavka);
                };
                PanelPogodnosti.Children.Add(stavka);
            }
        }

        private void PrimeniDugme_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(KapacitetTekst.Text) || string.IsNullOrWhiteSpace(CenaTekst.Text) ||
                string.IsNullOrWhiteSpace(NazivSobeTekst.Text) || string.IsNullOrWhiteSpace(OpisTekst.Text))
            {
                ErrorTekstBlok.Text = "Sva polja moraju biti popunjena.";
                return;
            }
            if (!int.TryParse(KapacitetTekst.Text, out int capacity) || capacity <= 0)
            {
                ErrorTekstBlok.Text = "Kapacitet mora biti pozitivan broj.";
                return;
            }
            if (!decimal.TryParse(CenaTekst.Text, out decimal price) || price <= 0)
            {
                ErrorTekstBlok.Text = "Cena mora biti pozitivan broj.";
                return;
            }
            ErrorTekstBlok.Text = string.Empty;

            string novoIme = NazivSobeTekst.Text;
            string noviOpis = OpisTekst.Text;
            int noviKapacitet = int.Parse(KapacitetTekst.Text);
            decimal novaCena = decimal.Parse(CenaTekst.Text);

            MenadzerBazePodataka.IzmeniSobu(SobaId, novoIme, noviKapacitet, novaCena, noviOpis);

            KapacitetTekst.BorderBrush = Brushes.Gray;
            CenaTekst.BorderBrush = Brushes.Gray;
            NazivSobeTekst.BorderBrush = Brushes.Gray;
            OpisTekst.BorderBrush = Brushes.Gray;
        }

        private void ZameniSliku_Click(object sender, RoutedEventArgs e)
        {
            var dijalogFajla = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };
            if (dijalogFajla.ShowDialog() == true)
            {
                byte[] bajtoviSlike = File.ReadAllBytes(dijalogFajla.FileName);
                SlikaSobe.Source = MenadzerResursa.IzvorOdNizaBajtova(bajtoviSlike);
                MenadzerBazePodataka.PromeniSlikuSobe(SobaId, bajtoviSlike);
            }
            else
            {
                ErrorTekstBlok.Text = "Neuspesno ucitavanje slike.";
            }
        }

        private void DodajPogodnostDugme_Click(object sender, RoutedEventArgs e)
        {
            var svePogodnosti = MenadzerBazePodataka.UcitajSvePogodnosti();
            var postojeciId = new HashSet<int>();

            foreach (StavkaPogodnostiAdmin item in PanelPogodnosti.Children)
            {
                postojeciId.Add(item.PogodnostId);
            }

            var dostupne = svePogodnosti.Where(a => !postojeciId.Contains(a.Id)).ToList();

            IskacuciProzorPogodnostiLista.Children.Clear();

            if (!dostupne.Any())
            {
                IskacuciProzorPogodnostiLista.Children.Add(new TextBlock
                {
                    Text = "Nema pogodnosti za dodavanje.",
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(5)
                });
            }
            else
            {
                foreach (var pogodnost in dostupne)
                {
                    var dugme = new Button
                    {
                        Content = pogodnost.Ime,
                        Tag = pogodnost,
                        Margin = new Thickness(2),
                        Padding = new Thickness(5)
                    };
                    dugme.Click += (s, e) => DodajPogodnost(pogodnost);
                    IskacuciProzorPogodnostiLista.Children.Add(dugme);
                }
            }

            IskacuciProzorPogodnosti.IsOpen = true;
        }
        private void DodajPogodnost(Pogodnost izabranaPogodnost)
        {
            MenadzerBazePodataka.DodajPogodnostSobi(SobaId, izabranaPogodnost.Id);

            var novaStavka = new StavkaPogodnostiAdmin(izabranaPogodnost.Id, izabranaPogodnost.Ime, izabranaPogodnost.Ikonica);

            novaStavka.DataContext = novaStavka;
            novaStavka.Obrisano += (s, e) =>
            {
                MenadzerBazePodataka.ObrisiPogodnostIzSobe(SobaId, izabranaPogodnost.Id);
                PanelPogodnosti.Children.Remove(novaStavka);
            };

            PanelPogodnosti.Children.Add(novaStavka);
            IskacuciProzorPogodnosti.IsOpen = false;
        }
    }


}