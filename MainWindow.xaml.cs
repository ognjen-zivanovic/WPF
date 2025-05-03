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

            Room[] rooms = DatabaseManager.GetMatchingRooms(CheckInDatePicker.SelectedDate.Value, CheckOutDatePicker.SelectedDate.Value, selectedAdults, selectedChildren);


            foreach (var room in rooms)
            {
                Amenity[] amenities = DatabaseManager.GetAmenitiesForRoom(room.Id);
                AddRoomToUI(room, amenities);
            }
        }

        // private void AddRoomToUI(Room room, Amenity[] amenities = null)
        // {
        //     var border = new Border
        //     {
        //         Width = 750,
        //         Margin = new Thickness(10),
        //         BorderBrush = new SolidColorBrush(Color.FromRgb(136, 136, 136)),
        //         BorderThickness = new Thickness(1),
        //         Background = new SolidColorBrush(Color.FromRgb(240, 248, 255)),
        //         CornerRadius = new CornerRadius(10)
        //     };

        //     var stackPanel = new StackPanel { Margin = new Thickness(10) };
        //     var imageSource = DatabaseManager.LoadImageFromDatabase(room.Id);

        //     var price = CalculatePrice(room.PricePerNight);

        //     // Create the image
        //     var image = new Image
        //     {
        //         Source = imageSource,
        //         Stretch = Stretch.UniformToFill
        //     };

        //     // Create an overlay panel for amenity icons
        //     var amenityPanel = new StackPanel
        //     {
        //         Orientation = Orientation.Horizontal,
        //         HorizontalAlignment = HorizontalAlignment.Center,
        //         VerticalAlignment = VerticalAlignment.Bottom,
        //         Margin = new Thickness(0, 0, 0, 10),
        //         Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)) // semi-transparent background
        //     };


        //     foreach (var amenity in amenities)
        //     {
        //         var icon = new Image
        //         {
        //             Source = DatabaseManager.SourceFromByteArray(amenity.Icon),
        //             Width = 75,
        //             Height = 75,
        //             Margin = new Thickness(5, 0, 5, 0),
        //             ToolTip = amenity.Name
        //         };
        //         amenityPanel.Children.Add(icon);
        //     }

        //     // Create a grid to overlay the icons over the image
        //     var imageGrid = new Grid
        //     {
        //         Margin = new Thickness(0, 0, 0, 10)
        //     };
        //     imageGrid.Children.Add(image);
        //     imageGrid.Children.Add(amenityPanel);

        //     // Add the grid to the stackPanel
        //     stackPanel.Children.Add(imageGrid);


        //     // stackPanel.Children.Add(new TextBlock
        //     // {
        //     //     Text = room.Name ?? "N/A",
        //     //     FontSize = 20,
        //     //     FontWeight = FontWeights.Bold,
        //     //     Margin = new Thickness(0, 0, 0, 10)
        //     // });
        //     Grid grid = new Grid
        //     {
        //         Margin = new Thickness(0, 0, 0, 10)
        //     };

        //     // Add two default columns
        //     grid.ColumnDefinitions.Add(new ColumnDefinition()); // Left
        //     grid.ColumnDefinitions.Add(new ColumnDefinition()); // Right

        //     // StackPanel for text info (Capacity + Price)
        //     StackPanel textPanel = new StackPanel
        //     {
        //         Orientation = Orientation.Vertical
        //     };

        //     // Capacity
        //     textPanel.Children.Add(new TextBlock
        //     {
        //         Text = $"Capacity: {room.Capacity}",
        //         FontSize = 16,
        //         Margin = new Thickness(0, 0, 0, 5)
        //     });

        //     // Price (large, bold, standout)
        //     textPanel.Children.Add(new TextBlock
        //     {
        //         Text = $"{price}",
        //         FontSize = 24,
        //         FontWeight = FontWeights.Bold,
        //         Foreground = new SolidColorBrush(Colors.DarkGreen),
        //         Margin = new Thickness(0, 0, 0, 5)
        //     });

        //     // Add textPanel to Grid's first column
        //     Grid.SetColumn(textPanel, 0);
        //     grid.Children.Add(textPanel);

        //     // Button on the right
        //     Button bookButton = new Button
        //     {
        //         Content = "Book Room",
        //         Width = 100,
        //         Height = 30,
        //         Margin = new Thickness(20, 10, 0, 0),
        //         Background = new SolidColorBrush(Color.FromRgb(173, 216, 230)),
        //         HorizontalAlignment = HorizontalAlignment.Right
        //     };
        //     bookButton.DataContext = room.Id;
        //     bookButton.Click += (sender, e) =>
        //     {
        //         var button = sender as Button;
        //         if (button != null)
        //         {
        //             var roomId = button.DataContext.ToString(); // Retrieve the room ID from DataContext
        //             var checkoutWindow = new CheckoutWindow(int.Parse(roomId), CheckInDatePicker.SelectedDate.Value, CheckOutDatePicker.SelectedDate.Value, CalculatePrice(room.PricePerNight));
        //             checkoutWindow.Show();
        //         }
        //     };

        //     // Add button to second column
        //     Grid.SetColumn(bookButton, 1);
        //     grid.Children.Add(bookButton);

        //     // Add the full grid to the main stackPanel
        //     stackPanel.Children.Add(grid);

        //     border.Child = stackPanel;
        //     RoomsStackPanel.Children.Add(border);


        //     // var border = new Border
        //     // {
        //     //     Width = 300,
        //     //     Height = 500,
        //     //     Margin = new Thickness(10),
        //     //     BorderBrush = new SolidColorBrush(Color.FromRgb(136, 136, 136)),
        //     //     BorderThickness = new Thickness(1),
        //     //     Background = new SolidColorBrush(Color.FromRgb(240, 248, 255)),
        //     //     CornerRadius = new CornerRadius(10)
        //     // };

        //     // var stackPanel = new StackPanel { Margin = new Thickness(10) };
        //     // var roomNumber = 0;
        //     // var imageSource = LoadImageFromDatabase(int.TryParse(reader["id"].ToString(), out roomNumber) ? roomNumber : 0);

        //     // // Parse price per night from the reader
        //     // float.TryParse(reader["price_per_night"]?.ToString(), out var pricePerNight);
        //     // var price = GetPrice(pricePerNight);

        //     // stackPanel.Children.Add(new Image
        //     // {
        //     //     Source = imageSource,
        //     //     Margin = new Thickness(0, 0, 0, 10)
        //     // });
        //     // stackPanel.Children.Add(new TextBlock
        //     // {
        //     //     Text = reader["name"]?.ToString() ?? "N/A",
        //     //     FontSize = 20,
        //     //     FontWeight = FontWeights.Bold,
        //     //     Margin = new Thickness(0, 0, 0, 10)
        //     // });
        //     // stackPanel.Children.Add(new TextBlock
        //     // {
        //     //     Text = $"Capacity: {reader["capacity"]}",
        //     //     FontSize = 16,
        //     //     Margin = new Thickness(0, 0, 0, 10)
        //     // });
        //     // stackPanel.Children.Add(new TextBlock
        //     // {
        //     //     Text = $"{price}",
        //     //     FontSize = 16,
        //     //     Margin = new Thickness(0, 0, 0, 10)
        //     // });
        //     // stackPanel.Children.Add(new TextBlock
        //     // {
        //     //     Text = $"Room ID: {reader["id"]}",
        //     //     FontSize = 16,
        //     //     Margin = new Thickness(0, 0, 0, 10)
        //     // });

        //     // Button bookButton = new Button
        //     // {
        //     //     Content = "Book Room",
        //     //     Width = 100,
        //     //     Height = 30,
        //     //     Background = new SolidColorBrush(Color.FromRgb(173, 216, 230)),
        //     // };

        //     // bookButton.DataContext = reader["id"];

        //     // bookButton.Click += (sender, e) =>
        //     // {
        //     //     var button = sender as Button;
        //     //     if (button != null)
        //     //     {
        //     //         var roomId = button.DataContext.ToString(); // Retrieve the room ID from DataContext
        //     //         var checkoutWindow = new CheckoutWindow(int.Parse(roomId), CheckInDatePicker.SelectedDate.Value, CheckOutDatePicker.SelectedDate.Value, CalculatePrice(pricePerNight));
        //     //         checkoutWindow.Show();
        //     //     }
        //     // };

        //     // stackPanel.Children.Add(bookButton);

        //     // border.Child = stackPanel;
        //     // RoomsStackPanel.Children.Add(border);
        // }

        private void AddRoomToUI(Room room, Amenity[] amenities)
        {
            var image = DatabaseManager.LoadImageFromDatabase(room.Id);
            var price = CalculatePrice(room.PricePerNight);

            var roomCard = new RoomCard();
            roomCard.SetRoomData(room, amenities, image, price, CheckInDatePicker.SelectedDate.Value, CheckOutDatePicker.SelectedDate.Value);

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
            return totalPrice;
        }

    }
}
