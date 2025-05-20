using System.Windows;

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
