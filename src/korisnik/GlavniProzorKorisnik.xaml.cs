using System.Windows;
using System.Windows.Controls;

namespace HotelRezervacije
{
    public partial class GlavniProzorKorisnik : Window
    {
        public GlavniProzorKorisnik()
        {
            InitializeComponent();

            DatumPrijaveKalendar.SelectedDate = DateTime.Now;
            DatumOdlaskaKalendar.SelectedDate = DateTime.Now.AddDays(1);
            PopuniPogodnosti();
        }

        private void PopuniPogodnosti()
        {
            Pogodnost[] pogodnosti = MenadzerBazePodataka.UcitajSvePogodnosti();
            foreach (var pogodnost in pogodnosti)
            {
                CheckBox checkBox = new CheckBox
                {
                    Content = pogodnost.Ime,
                    FontSize = 16,
                    Margin = new Thickness(0, 2, 0, 2)
                };
                PanelZaPogodnosti.Children.Add(checkBox);
            }
        }

        private string? ValidacijaPodataka()
        {
            if (DatumPrijaveKalendar.SelectedDate == null)
            {
                return "Molimo odaberite datum dolaska.";
            }
            if (DatumOdlaskaKalendar.SelectedDate == null)
            {
                return "Molimo odaberite datum odlaska.";
            }

            if (DatumPrijaveKalendar.SelectedDate >= DatumOdlaskaKalendar.SelectedDate)
            {
                return "Datum odlaska mora biti nakon datuma dolaska.";
            }

            if (ComboBoxOdraslih.SelectedItem == null)
            {
                return "Molimo odaberite broj odraslih.";
            }
            if (ComboBoxDece.SelectedItem == null)
            {
                return "Molimo odaberite broj dece.";
            }

            foreach (var comboBoxGodinaDeteta in PanelZaGodineDece.Children)
            {
                if (comboBoxGodinaDeteta is ComboBox comboBox && comboBox.SelectedItem == null)
                {
                    return "Molimo odaberite godinu svakog deteta.";
                }
            }

            return null;
        }

        private void PrikaziSobeDugme_Click(object sender, RoutedEventArgs e)
        {
            string? error = ValidacijaPodataka();
            if (error != null)
            {
                ErrorTekst.Text = error;
                ErrorTekst.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                ErrorTekst.Text = string.Empty;
                ErrorTekst.Visibility = Visibility.Collapsed;
            }

            PrikaziSobe();
        }

        private void PrikaziSobe()
        {
            string datumDolaska = DatumPrijaveKalendar.SelectedDate.Value.AddHours(14).ToString("yyyy-MM-dd HH:mm");
            string datumOdlaska = DatumOdlaskaKalendar.SelectedDate.Value.AddHours(12).ToString("yyyy-MM-dd HH:mm");

            int brojOdraslih = int.TryParse((ComboBoxOdraslih.SelectedItem as ComboBoxItem)?.Content as string, out brojOdraslih) ? brojOdraslih : 0;
            int brojDece = int.TryParse((ComboBoxDece.SelectedItem as ComboBoxItem)?.Content as string, out brojDece) ? brojDece : 0;
            int brojBeba = IzbrojBebe();
            int ukupnoGostiju = brojOdraslih + brojDece - brojBeba;
            int ukupnoBeba = brojBeba;

            List<string> izabranePogodnosti = new List<string>();
            foreach (var dete in PanelZaPogodnosti.Children)
            {
                if (dete is CheckBox checkBox && checkBox.IsChecked == true)
                {
                    izabranePogodnosti.Add(checkBox.Content.ToString());
                }
            }

            Soba[] sobe = MenadzerBazePodataka.UcitajOdgovarajuceSobe(DatumPrijaveKalendar.SelectedDate.Value, DatumOdlaskaKalendar.SelectedDate.Value, ukupnoGostiju, izabranePogodnosti.ToArray());


            PanelZaSobe.Children.Clear();
            foreach (var soba in sobe)
            {
                Pogodnost[] pogodnosti = MenadzerBazePodataka.UcitajPogodnostiZaSobu(soba.Id);
                DodajKarticuSobe(soba, pogodnosti, datumDolaska, datumOdlaska, ukupnoGostiju, ukupnoBeba);
            }
        }

        private void DodajKarticuSobe(Soba soba, Pogodnost[] pogodnosti, string datumDolaska, string datumOdlaska, int ukupnoGostiju, int ukupnoBeba)
        {
            var image = MenadzerBazePodataka.UcitajSlikuIzBazePodataka(soba.Id);
            var price = IzracunajCenu(soba.CenaPoNoci);

            var karticaSobe = new KarticaSobeKorisnik();
            karticaSobe.PostaviPodatke(soba, pogodnosti, image, price, datumDolaska, datumOdlaska, ukupnoGostiju, ukupnoBeba);

            PanelZaSobe.Children.Add(karticaSobe);
        }

        private void ComboBoxDece_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PanelZaGodineDece.Children.Clear();

            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem && int.TryParse((string)selectedItem.Content, out int brojDece))
            {
                if (brojDece == 0)
                {
                    GodineDeceTekst.Visibility = Visibility.Collapsed;
                }
                else
                {
                    GodineDeceTekst.Visibility = Visibility.Visible;
                }

                for (int i = 0; i < brojDece; i++)
                {
                    ComboBox childAgeComboBox = new ComboBox();
                    childAgeComboBox.Items.Add("<2");
                    childAgeComboBox.Items.Add("3-11");
                    childAgeComboBox.Items.Add(">12");
                    PanelZaGodineDece.Children.Add(childAgeComboBox);
                }
            }
        }

        private int IzbrojBebe()
        {
            int broj = 0;
            foreach (var comboBoxGodinaDeteta in PanelZaGodineDece.Children)
            {
                if (comboBoxGodinaDeteta is ComboBox comboBox && comboBox.SelectedItem != null)
                {
                    string godine = comboBox.SelectedItem.ToString();
                    if (godine == "<2")
                    {
                        broj++;
                    }
                }
            }
            return broj;
        }

        private decimal IzracunajCenu(decimal cenaPoNoci)
        {
            int brojOdraslih = int.TryParse(
                (ComboBoxOdraslih.SelectedItem as ComboBoxItem)?.Content as string,
                out var odrasli) ? odrasli : 0;

            int brojDece = int.TryParse(
                (ComboBoxDece.SelectedItem as ComboBoxItem)?.Content as string,
                out var deca) ? deca : 0;


            decimal ukupnaCena = 0;

            if (DatumPrijaveKalendar.SelectedDate is DateTime datumDolaska &&
                DatumOdlaskaKalendar.SelectedDate is DateTime datumOdlaska)
            {
                int ukupnoDana = (int)(datumOdlaska - datumDolaska).TotalDays;

                ukupnaCena += cenaPoNoci * ukupnoDana * brojOdraslih;

                for (int i = 0; i < brojDece; i++)
                {
                    if (PanelZaGodineDece.Children[i] is ComboBox comboBoxGodinaDeteta &&
                        comboBoxGodinaDeteta.SelectedItem != null)
                    {
                        string godine = comboBoxGodinaDeteta.SelectedItem.ToString();
                        decimal popust = godine switch
                        {
                            "<2" => 0.0m,
                            "3-11" => 0.5m,
                            ">12" => 1.0m,
                            _ => 0.0m
                        };

                        ukupnaCena += cenaPoNoci * ukupnoDana * popust;
                    }
                }
            }
            return (decimal)Math.Round(ukupnaCena, 2);
        }
    }
}
