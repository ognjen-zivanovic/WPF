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
    public partial class GlavniProzorKorisnik : Window
    {
        public GlavniProzorKorisnik()
        {
            InitializeComponent();

            DatumPrijaveKalendar.SelectedDate = DateTime.Now;
            DatumOdlaskaKalendar.SelectedDate = DateTime.Now.AddDays(1);
            PopulateAmenities();
        }

        private void PopulateAmenities()
        {
            Amenity[] pogodnosti = DatabaseManager.GetAllAmenities();
            foreach (var amenity in pogodnosti)
            {
                CheckBox checkBox = new CheckBox
                {
                    Content = amenity.Name,
                    FontSize = 16,
                    Margin = new Thickness(0, 2, 0, 2)
                };
                PanelZaPogodnosti.Children.Add(checkBox);
            }
        }

        private string? InfoValidation()
        {
            if (DatumPrijaveKalendar.SelectedDate == null)
            {
                return "Please select a check-in date.";
            }
            if (DatumOdlaskaKalendar.SelectedDate == null)
            {
                return "Please select a check-out date.";
            }

            if (DatumPrijaveKalendar.SelectedDate >= DatumOdlaskaKalendar.SelectedDate)
            {
                return "Check-out date must be after check-in date.";
            }

            if (KombinovaniOdabirOdraslih.SelectedItem == null)
            {
                return "Please select the number of adults.";
            }
            if (KombinovaniOdabirDece.SelectedItem == null)
            {
                return "Please select the number of children.";
            }

            foreach (var childAgeComboBox in PanelZaUzrasteDece.Children)
            {
                if (childAgeComboBox is ComboBox comboBox && comboBox.SelectedItem == null)
                {
                    return "Please select the age of each child.";
                }
            }

            return null;
        }

        private void ShowRooms_Button_Click(object sender, RoutedEventArgs e)
        {
            string? error = InfoValidation();
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


            ShowRooms();
        }

        private void ShowRooms()
        {
            string checkInDate = DatumPrijaveKalendar.SelectedDate.Value.AddHours(14).ToString("yyyy-MM-dd HH:mm");
            string checkOutDate = DatumOdlaskaKalendar.SelectedDate.Value.AddHours(12).ToString("yyyy-MM-dd HH:mm");

            int brojOdraslih = int.TryParse((KombinovaniOdabirOdraslih.SelectedItem as ComboBoxItem)?.Content as string, out brojOdraslih) ? brojOdraslih : 0;
            int brojDece = int.TryParse((KombinovaniOdabirDece.SelectedItem as ComboBoxItem)?.Content as string, out brojDece) ? brojDece : 0;
            int numBabies = CountBabies();
            int ukupnoGostiju = brojOdraslih + brojDece - numBabies;
            int ukupnoBeba = numBabies;

            List<string> izabranePogodnosti = new List<string>();
            foreach (var child in PanelZaPogodnosti.Children)
            {
                if (child is CheckBox checkBox && checkBox.IsChecked == true)
                {
                    izabranePogodnosti.Add(checkBox.Content.ToString());
                }
            }

            Room[] sobe = DatabaseManager.GetMatchingRooms(DatumPrijaveKalendar.SelectedDate.Value, DatumOdlaskaKalendar.SelectedDate.Value, ukupnoGostiju, izabranePogodnosti.ToArray());

            ClearRoomsUI();
            foreach (var soba in sobe)
            {
                Amenity[] pogodnosti = DatabaseManager.GetAmenitiesForRoom(soba.Id);
                AddRoomToUI(soba, pogodnosti, checkInDate, checkOutDate, ukupnoGostiju, ukupnoBeba);
            }
        }
        private void ClearRoomsUI()
        {
            PanelZaSobe.Children.Clear();
        }

        private void AddRoomToUI(Room soba, Amenity[] pogodnosti, string checkInDate, string checkOutDate, int ukupnoGostiju, int ukupnoBeba)
        {
            var image = DatabaseManager.LoadImageFromDatabase(soba.Id);
            var price = CalculatePrice(soba.PricePerNight);

            var karticaSobe = new KarticaSobeKorisnik();
            karticaSobe.SetRoomData(soba, pogodnosti, image, price, checkInDate, checkOutDate, ukupnoGostiju, ukupnoBeba);

            PanelZaSobe.Children.Add(karticaSobe);
        }

        private void KombinovaniOdabirDece_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PanelZaUzrasteDece.Children.Clear();
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedItem is ComboBoxItem selectedItem && int.TryParse((string)selectedItem.Content, out int selectedValue))
                {
                    if (selectedValue == 0)
                    {
                        ChildrenAgesText.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        ChildrenAgesText.Visibility = Visibility.Visible;
                    }
                    for (int i = 0; i < selectedValue; i++)
                    {
                        ComboBox childAgeComboBox = new ComboBox();
                        childAgeComboBox.Name = $"Child{i + 1}_ComboBox";
                        childAgeComboBox.Items.Add("<2");
                        childAgeComboBox.Items.Add("3-11");
                        childAgeComboBox.Items.Add(">12");
                        PanelZaUzrasteDece.Children.Add(childAgeComboBox);
                    }
                }
            }
        }

        private int CountBabies()
        {
            int count = 0;
            foreach (var childAgeComboBox in PanelZaUzrasteDece.Children)
            {
                if (childAgeComboBox is ComboBox comboBox && comboBox.SelectedItem != null)
                {
                    string ageGroup = comboBox.SelectedItem.ToString();
                    if (ageGroup == "<2")
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private decimal CalculatePrice(decimal pricePerNight)
        {
            int numAdults = int.TryParse(
                (KombinovaniOdabirOdraslih.SelectedItem as ComboBoxItem)?.Content as string,
                out var adults) ? adults : 0;

            int numChildren = int.TryParse(
                (KombinovaniOdabirDece.SelectedItem as ComboBoxItem)?.Content as string,
                out var children) ? children : 0;


            decimal totalPrice = 0;

            if (DatumPrijaveKalendar.SelectedDate is DateTime checkIn &&
                DatumOdlaskaKalendar.SelectedDate is DateTime checkOut)
            {
                int totalDays = (int)(checkOut - checkIn).TotalDays;

                totalPrice += pricePerNight * totalDays * numAdults;

                for (int i = 0; i < numChildren; i++)
                {
                    if (PanelZaUzrasteDece.Children[i] is ComboBox childAgeCombo &&
                        childAgeCombo.SelectedItem != null)
                    {
                        string ageGroup = childAgeCombo.SelectedItem.ToString();
                        decimal multiplier = ageGroup switch
                        {
                            "<2" => 0.0m,
                            "3-11" => 0.5m,
                            ">12" => 1.0m,
                            _ => 0.0m
                        };

                        totalPrice += pricePerNight * totalDays * multiplier;
                    }
                }
            }
            return (decimal)Math.Round(totalPrice, 2);
        }

        private void ShowAdminWindowButton_Click(object sender, RoutedEventArgs e)
        {
            GlavniProzorAdmin GlavniProzorAdmin = new GlavniProzorAdmin();
            GlavniProzorAdmin.Show();
            this.Close();
        }
    }
}
