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
    public partial class ProzorPogodnostiAdmin : Window
    {
        public ProzorPogodnostiAdmin()
        {
            InitializeComponent();
            PrikaziPogodnosti();
        }

        private void PrikaziPogodnosti()
        {
            FontFamily unikodFont = (FontFamily)Application.Current.Resources["UnicodeFont"];
            PanelZaPogodnosti.Children.Clear();

            Pogodnost[] pogodnosti = DatabaseManager.UcitajSvePogodnosti();
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
            DatabaseManager.DodajPogodnost(novaPogodnost);
            PrikaziPogodnosti();
        }

        private bool ObrisiStavkuPogodnosti(int pogodnostId)
        {
            Soba[] sobe = DatabaseManager.UcitajSobePoIdPogodnosti(pogodnostId);
            if (sobe.Length > 0)
            {
                MessageBox.Show("Nije moguce obrisati ovu pogodnost jer je povezana sa nekom sobom.");
                return false;
            }
            DatabaseManager.ObrisiPogodnost(pogodnostId);
            return true;
        }
    }
}