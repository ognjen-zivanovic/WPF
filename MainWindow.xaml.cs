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

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DatabaseManager.Init();
            // set default dates for check-in and check-out
            CheckInDatePicker.SelectedDate = DateTime.Now;
            CheckOutDatePicker.SelectedDate = DateTime.Now.AddDays(1);
            PopulateAmenities();
        }

        private void PopulateAmenities()
        {
            // Assuming you have a method to get amenities from the database
            Amenity[] amenities = DatabaseManager.GetAllAmenities();
            foreach (var amenity in amenities)
            {
                CheckBox checkBox = new CheckBox
                {
                    Content = amenity.Name,
                    FontSize = 16,
                    FontFamily = new FontFamily("Arial"),
                    Margin = new Thickness(0, 2, 0, 2)
                };
                AmenitiesStackPanel.Children.Add(checkBox);
            }
        }

        private string? InfoValidation()
        {
            if (CheckInDatePicker.SelectedDate == null)
            {
                return "Please select a check-in date.";
            }
            if (CheckOutDatePicker.SelectedDate == null)
            {
                return "Please select a check-out date.";
            }

            if (CheckInDatePicker.SelectedDate >= CheckOutDatePicker.SelectedDate)
            {
                return "Check-out date must be after check-in date.";
            }

            if (Adults_ComboBox.SelectedItem == null)
            {
                return "Please select the number of adults.";
            }
            if (Children_ComboBox.SelectedItem == null)
            {
                return "Please select the number of children.";
            }

            foreach (var childAgeComboBox in ChildrenAgesStackPanel.Children)
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
                ErrorText.Text = error;
                ErrorText.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                ErrorText.Text = string.Empty;
                ErrorText.Visibility = Visibility.Collapsed;
            }


            ShowRooms();
        }

        private void ShowRooms()
        {
            string checkInDate = CheckInDatePicker.SelectedDate.Value.ToString("yyyy-MM-dd");
            string checkOutDate = CheckOutDatePicker.SelectedDate.Value.ToString("yyyy-MM-dd");

            int selectedAdults = int.TryParse((string)((ComboBoxItem)Adults_ComboBox.SelectedItem)?.Content, out selectedAdults) ? selectedAdults : 0;
            int selectedChildren = int.TryParse((string)((ComboBoxItem)Children_ComboBox.SelectedItem)?.Content, out selectedChildren) ? selectedChildren : 0;

            List<string> selectedAmenities = new List<string>();
            foreach (var child in AmenitiesStackPanel.Children)
            {
                if (child is CheckBox checkBox && checkBox.IsChecked == true)
                {
                    selectedAmenities.Add(checkBox.Content.ToString());
                }
            }

            Room[] rooms = DatabaseManager.GetMatchingRooms(CheckInDatePicker.SelectedDate.Value, CheckOutDatePicker.SelectedDate.Value, selectedAdults, selectedChildren, selectedAmenities.ToArray());

            ClearRoomsUI();
            foreach (var room in rooms)
            {
                Amenity[] amenities = DatabaseManager.GetAmenitiesForRoom(room.Id);
                AddRoomToUI(room, amenities);
            }
        }
        private void ClearRoomsUI()
        {
            RoomsStackPanel.Children.Clear();
        }

        private void AddRoomToUI(Room room, Amenity[] amenities)
        {
            var image = DatabaseManager.LoadImageFromDatabase(room.Id);
            var price = CalculatePrice(room.PricePerNight);

            var roomCard = new RoomCard();
            roomCard.SetRoomData(room, amenities, image, price);

            RoomsStackPanel.Children.Add(roomCard);
        }

        private void Children_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChildrenAgesStackPanel.Children.Clear();
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedItem is ComboBoxItem selectedItem && int.TryParse((string)selectedItem.Content, out int selectedValue))
                {
                    for (int i = 0; i < selectedValue; i++)
                    {
                        ComboBox childAgeComboBox = new ComboBox();
                        childAgeComboBox.Name = $"Child{i + 1}_ComboBox";
                        childAgeComboBox.Items.Add("<2");
                        childAgeComboBox.Items.Add("3-11");
                        childAgeComboBox.Items.Add(">12");
                        ChildrenAgesStackPanel.Children.Add(childAgeComboBox);
                    }
                }
            }
        }

        private float CalculatePrice(float pricePerNight)
        {
            // Parse number of adults
            int numAdults = int.TryParse(
                (Adults_ComboBox.SelectedItem as ComboBoxItem)?.Content as string,
                out var adults) ? adults : 0;

            // Parse number of children
            int numChildren = int.TryParse(
                (Children_ComboBox.SelectedItem as ComboBoxItem)?.Content as string,
                out var children) ? children : 0;


            float totalPrice = 0;

            if (CheckInDatePicker.SelectedDate is DateTime checkIn &&
                CheckOutDatePicker.SelectedDate is DateTime checkOut)
            {
                int totalDays = (int)(checkOut - checkIn).TotalDays;

                // Base price for adults
                totalPrice += pricePerNight * totalDays * numAdults;

                // Price adjustment for children
                for (int i = 0; i < numChildren; i++)
                {
                    if (ChildrenAgesStackPanel.Children[i] is ComboBox childAgeCombo &&
                        childAgeCombo.SelectedItem != null)
                    {
                        string ageGroup = childAgeCombo.SelectedItem.ToString();
                        float multiplier = ageGroup switch
                        {
                            "<2" => 0.0f,
                            "3-11" => 0.5f,
                            ">12" => 1.0f,
                            _ => 0.0f
                        };

                        totalPrice += pricePerNight * totalDays * multiplier;
                    }
                }
            }
            return (float)Math.Round(totalPrice, 2);
        }

        private void ShowAdminWindowButton_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow();
            adminWindow.Show();
            this.Close();
        }
    }
}
