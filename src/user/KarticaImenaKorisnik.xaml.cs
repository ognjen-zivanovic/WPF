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

    public partial class KarticaImenaKorisnik : UserControl
    {
        public string GuestName { get; set; }
        public string GuestSurname { get; set; }

        public KarticaImenaKorisnik()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}