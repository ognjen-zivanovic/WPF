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
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace HotelRezervacije
{
    public partial class ProzorUspesneRezervacije : Window
    {
        public int RezervacijaId { get; set; }

        public ProzorUspesneRezervacije()
        {
            InitializeComponent();
        }

        public ProzorUspesneRezervacije(int rezervacijaId)
        {
            InitializeComponent();

            RezervacijaId = rezervacijaId;
            this.DataContext = this;
        }
    }
}
