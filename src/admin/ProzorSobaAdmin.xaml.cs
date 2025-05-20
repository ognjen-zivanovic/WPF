using System.Windows;

namespace HotelRezervacije
{
    public partial class ProzorSobaAdmin : Window
    {
        public ProzorSobaAdmin()
        {
            InitializeComponent();
            PrikaziSobe();
        }

        private void PrikaziSobe()
        {
            AdminPanelZaSobe.Children.Clear();

            Soba[] sobe = MenadzerBazePodataka.UcitajSveSobe();
            foreach (var soba in sobe)
            {
                var slika = MenadzerBazePodataka.UcitajSlikuIzBazePodataka(soba.Id);
                var cena = soba.CenaPoNoci;

                KarticaSobeAdmin karticaSobe = new KarticaSobeAdmin();
                Pogodnost[] pogodnosti = MenadzerBazePodataka.UcitajPogodnostiZaSobu(soba.Id);
                karticaSobe.PostaviPodatkeSobe(soba, pogodnosti, slika);
                karticaSobe.ObrisiDugme.Click += (s, e) =>
                {
                    if (ObrisiSobu(soba.Id))
                    {
                        AdminPanelZaSobe.Children.Remove(karticaSobe);
                    }
                };
                AdminPanelZaSobe.Children.Add(karticaSobe);
            }
        }

        private void DodajSobuDugme_Click(object sender, RoutedEventArgs e)
        {
            Soba novaSoba = new Soba
            {
                Ime = "Nova Soba",
                Kapacitet = 2,
                CenaPoNoci = 100.00m,
                Opis = "Opis nove sobe."
            };
            int dodataSobaId = MenadzerBazePodataka.DodajSobu(novaSoba);
            MenadzerBazePodataka.DodajSliku(dodataSobaId, (byte[])null);
            PrikaziSobe();
        }


        private bool ObrisiSobu(int sobaId)
        {
            Rezervacija[] rezervacije = MenadzerBazePodataka.UcitajRezervacijeZaSobu(sobaId);
            if (rezervacije.Length > 0)
            {
                MessageBox.Show("Nije moguce obrisati sobu jer je povezana sa nekom rezervacijom.");
                return false;
            }

            MenadzerBazePodataka.ObrisiSobu(sobaId);
            MenadzerBazePodataka.ObrisiSveSobeSaIdSobe(sobaId);
            MenadzerBazePodataka.ObrisiSvePogodnostiIzSobe(sobaId);
            return true;
        }
    }
}