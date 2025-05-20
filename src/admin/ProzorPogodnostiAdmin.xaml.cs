using System.Windows;

namespace HotelRezervacije
{
    public partial class ProzorPogodnostiAdmin : Window
    {
        public ProzorPogodnostiAdmin()
        {
            InitializeComponent();
            PrikaziPogodnosti();
        }

        private void PrikaziPogodnosti()
        {
            PanelZaPogodnosti.Children.Clear();

            Pogodnost[] pogodnosti = MenadzerBazePodataka.UcitajSvePogodnosti();
            foreach (var pogodnost in pogodnosti)
            {
                StavkaMenjivePogodnostiAdmin karticaPogodnosti = new StavkaMenjivePogodnostiAdmin(pogodnost.Id, pogodnost.Ime, pogodnost.Ikonica);

                karticaPogodnosti.ObrisiDugme.Click += (s, e) =>
                {
                    if (ObrisiStavkuPogodnosti(pogodnost.Id))
                    {
                        PanelZaPogodnosti.Children.Remove(karticaPogodnosti);
                    }
                };

                PanelZaPogodnosti.Children.Add(karticaPogodnosti);
            }
        }


        private void DodajPogodnostDugme_Click(object sender, RoutedEventArgs e)
        {
            Pogodnost novaPogodnost = new Pogodnost
            {
                Ime = "Nova Pogodnost",
                Ikonica = "",
            };
            MenadzerBazePodataka.DodajPogodnost(novaPogodnost);
            PrikaziPogodnosti();
        }

        private bool ObrisiStavkuPogodnosti(int pogodnostId)
        {
            Soba[] sobe = MenadzerBazePodataka.UcitajSobePoIdPogodnosti(pogodnostId);
            if (sobe.Length > 0)
            {
                MessageBox.Show("Nije moguce obrisati ovu pogodnost jer je povezana sa nekom sobom.");
                return false;
            }
            MenadzerBazePodataka.ObrisiPogodnost(pogodnostId);
            return true;
        }
    }
}