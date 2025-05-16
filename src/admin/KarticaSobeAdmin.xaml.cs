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

namespace HotelRezervacije
{

    public partial class KarticaSobeAdmin : UserControl
    {
        public int RoomId { get; set; }

        private bool changed = false;

        public KarticaSobeAdmin()
        {
            InitializeComponent();
        }

        public void SetRoomData(Room room, Amenity[] amenities, ImageSource imageSource)
        {
            RoomId = room.Id;

            if (imageSource != null)
            {
                RoomImage.Source = imageSource;
            }
            else
            {
                RoomImage.Source = DatabaseManager.SourceFromFileName("icons/no-image.png");
            }
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


            SwapImage.Source = DatabaseManager.SourceFromFileName("icons/change-icon.png");

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
                var item = new StavkaPogodnostiAdmin(amenity.Id, amenity.Name, amenity.Icon);
                // item.SetImageSource(DatabaseManager.SourceFromByteArray(amenity.Icon));
                item.Deleted += (s, e) =>
                {
                    DatabaseManager.DeleteAmenityFromRoom(room.Id, amenity.Id);
                    AmenityPanel.Children.Remove(item);
                };
                AmenityPanel.Children.Add(item);
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (changed)
            {
                if (string.IsNullOrWhiteSpace(CapacityText.Text) || string.IsNullOrWhiteSpace(PriceText.Text) ||
                    string.IsNullOrWhiteSpace(RoomNameText.Text) || string.IsNullOrWhiteSpace(DescriptionText.Text))
                {
                    ErrorTekstBlock.Text = "All fields must be filled.";
                    return;
                }
                if (!int.TryParse(CapacityText.Text, out int capacity) || capacity <= 0)
                {
                    ErrorTekstBlock.Text = "Capacity must be a positive integer.";
                    return;
                }
                if (!decimal.TryParse(PriceText.Text, out decimal price) || price <= 0)
                {
                    ErrorTekstBlock.Text = "Price must be a positive number.";
                    return;
                }
                ErrorTekstBlock.Text = string.Empty;

                int roomId = int.Parse(CapacityText.DataContext.ToString());
                int newCapacity = int.Parse(CapacityText.Text);
                decimal newPrice = decimal.Parse(PriceText.Text);
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
                ErrorTekstBlock.Text = "Failed to load image.";
            }
        }

        private void AddAmenityButton_Click(object sender, RoutedEventArgs e)
        {
            var allAmenities = DatabaseManager.GetAllAmenities();
            var existingIds = new HashSet<int>();

            foreach (StavkaPogodnostiAdmin item in AmenityPanel.Children.OfType<StavkaPogodnostiAdmin>())
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

                var newItem = new StavkaPogodnostiAdmin(selected.Id, selected.Name, selected.Icon);
                newItem.DataContext = newItem;
                // newItem.SetImageSource(DatabaseManager.SourceFromByteArray(selected.Icon));
                newItem.Deleted += (s2, e2) =>
                {
                    DatabaseManager.DeleteAmenityFromRoom(RoomId, selected.Id);
                    AmenityPanel.Children.Remove(newItem);
                };

                AmenityPanel.Children.Add(newItem);
                AmenityPopup.IsOpen = false;
            }
        }
    }


}