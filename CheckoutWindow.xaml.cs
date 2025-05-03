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
    public partial class CheckoutWindow : Window
    {

        private int _roomId;
        private DateTime _checkInDate;
        private DateTime _checkOutDate;
        private float _totalPrice;

        public CheckoutWindow()
        {
            InitializeComponent();
        }

        public CheckoutWindow(int roomId, DateTime checkInDate, DateTime checkOutDate, float totalPrice)
        {
            InitializeComponent();

            _roomId = roomId;
            _checkInDate = checkInDate;
            _checkOutDate = checkOutDate;
            _totalPrice = totalPrice;

            RoomIdTextBlock.Text = $"Room ID: {roomId}";
            CheckInDateTextBlock.Text = $"Check-In Date: {checkInDate:yyyy-MM-dd}";
            CheckOutDateTextBlock.Text = $"Check-Out Date: {checkOutDate:yyyy-MM-dd}";
            TotalPriceTextBlock.Text = $"Total Price: {totalPrice:C2}";

            RoomImage.Source = DatabaseManager.LoadImageFromDatabase(roomId);
        }
    }
}
