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

namespace WpfApp1
{

    public partial class AdminRoomCard : UserControl
    {
        public int RoomId { get; set; }

        private bool changed = false;

        public AdminRoomCard()
        {
            InitializeComponent();
        }

        public void SetRoomData(Room room, Amenity[] amenities, ImageSource imageSource)
        {
            RoomId = room.Id;

            RoomImage.Source = imageSource;
            RoomImage.MouseEnter += (s, e) =>
            {
                SwapImage.Visibility = Visibility.Visible;
            };
            RoomImage.MouseLeave += (s, e) =>
            {
                SwapImage.Visibility = Visibility.Hidden;
            };

            SwapImage.MouseEnter += (s, e) =>
            {
                SwapImage.Visibility = Visibility.Visible;
            };

            byte[] imageBytes = File.ReadAllBytes("icons/change-icon.png");
            SwapImage.Source = DatabaseManager.SourceFromByteArray(imageBytes);

            CapacityText.Text = $"{room.Capacity}";
            PriceText.Text = $"{room.PricePerNight}";
            RoomNameText.Text = room.Name;
            DescriptionText.Text = room.Description;

            CapacityText.DataContext = room.Id;
            PriceText.DataContext = room.Id;
            RoomNameText.DataContext = room.Id;
            DescriptionText.DataContext = room.Id;

            CapacityText.BorderBrush = Brushes.Gray;
            PriceText.BorderBrush = Brushes.Gray;
            RoomNameText.BorderBrush = Brushes.Gray;
            DescriptionText.BorderBrush = Brushes.Gray;

            CapacityText.TextChanged += (s, e) =>
            {
                CapacityText.BorderBrush = Brushes.Black;
                changed = true;
            };

            PriceText.TextChanged += (s, e) =>
            {
                PriceText.BorderBrush = Brushes.Black;
                changed = true;
            };
            RoomNameText.TextChanged += (s, e) =>
            {
                RoomNameText.BorderBrush = Brushes.Black;
                changed = true;
            };
            DescriptionText.TextChanged += (s, e) =>
            {
                DescriptionText.BorderBrush = Brushes.Black;
                changed = true;
            };


            AmenityPanel.Children.Clear();
            foreach (var amenity in amenities)
            {
                var item = new AdminAmenityItem
                {
                    AmenityId = amenity.Id,
                    AmenityName = amenity.Name
                };
                item.SetImageSource(DatabaseManager.SourceFromByteArray(amenity.Icon));
                item.Deleted += (s, e) =>
                {
                    DatabaseManager.DeleteAmenityFromRoom(room.Id, amenity.Id);
                    AmenityPanel.Children.Remove(item);
                };
                AmenityPanel.Children.Add(item);
            }

            var addButton = new Button
            {
                Content = "+",
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                Foreground = Brushes.Green,
                FontSize = 32,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5)
            };
            addButton.Click += AddAmenityButton_Click;
            AmenityPanel.Children.Add(addButton);

        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (changed)
            {
                if (string.IsNullOrWhiteSpace(CapacityText.Text) || string.IsNullOrWhiteSpace(PriceText.Text) ||
                    string.IsNullOrWhiteSpace(RoomNameText.Text) || string.IsNullOrWhiteSpace(DescriptionText.Text))
                {
                    ErrorTextBlock.Text = "All fields must be filled.";
                    return;
                }
                if (!int.TryParse(CapacityText.Text, out int capacity) || capacity <= 0)
                {
                    ErrorTextBlock.Text = "Capacity must be a positive integer.";
                    return;
                }
                if (!float.TryParse(PriceText.Text, out float price) || price <= 0)
                {
                    ErrorTextBlock.Text = "Price must be a positive number.";
                    return;
                }
                ErrorTextBlock.Text = string.Empty;

                int roomId = int.Parse(CapacityText.DataContext.ToString());
                int newCapacity = int.Parse(CapacityText.Text);
                float newPrice = float.Parse(PriceText.Text);
                string newName = RoomNameText.Text;
                string newDescription = DescriptionText.Text;

                DatabaseManager.UpdateRoom(roomId, newName, newCapacity, newPrice, newDescription);
                changed = false;

                CapacityText.BorderBrush = Brushes.Gray;
                PriceText.BorderBrush = Brushes.Gray;
                RoomNameText.BorderBrush = Brushes.Gray;
                DescriptionText.BorderBrush = Brushes.Gray;
            }
        }

        private void SwapImage_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };
            if (fileDialog.ShowDialog() == true)
            {
                byte[] imageBytes = File.ReadAllBytes(fileDialog.FileName);
                RoomImage.Source = DatabaseManager.SourceFromByteArray(imageBytes);
                DatabaseManager.UpdateRoomImage(RoomId, imageBytes);
            }
            else
            {
                ErrorTextBlock.Text = "Failed to load image.";
            }
        }

        private void AddAmenityButton_Click(object sender, RoutedEventArgs e)
        {
            var allAmenities = DatabaseManager.GetAllAmenities();
            var existingIds = new HashSet<int>();

            foreach (AdminAmenityItem item in AmenityPanel.Children.OfType<AdminAmenityItem>())
            {
                existingIds.Add(item.AmenityId);
            }

            var available = allAmenities.Where(a => !existingIds.Contains(a.Id)).ToList();

            AmenityPopupList.Children.Clear();

            if (!available.Any())
            {
                AmenityPopupList.Children.Add(new TextBlock
                {
                    Text = "No amenities left to add.",
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(5)
                });
            }
            else
            {
                foreach (var amenity in available)
                {
                    var btn = new Button
                    {
                        Content = amenity.Name,
                        Tag = amenity,
                        Margin = new Thickness(2),
                        Padding = new Thickness(5)
                    };
                    btn.Click += AmenityOption_Click;
                    AmenityPopupList.Children.Add(btn);
                }
            }

            AmenityPopup.IsOpen = true;
        }
        private void AmenityOption_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Amenity selected)
            {
                DatabaseManager.AddAmenityToRoom(RoomId, selected.Id);

                var newItem = new AdminAmenityItem
                {
                    AmenityId = selected.Id,
                    AmenityName = selected.Name
                };
                newItem.SetImageSource(DatabaseManager.SourceFromByteArray(selected.Icon));
                newItem.Deleted += (s2, e2) =>
                {
                    DatabaseManager.DeleteAmenityFromRoom(RoomId, selected.Id);
                    AmenityPanel.Children.Remove(newItem);
                };

                // Insert before the "+" button
                AmenityPanel.Children.Insert(AmenityPanel.Children.Count - 1, newItem);
                AmenityPopup.IsOpen = false;
            }
        }

    }
}