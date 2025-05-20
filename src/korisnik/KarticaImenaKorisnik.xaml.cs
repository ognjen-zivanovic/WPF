using System.Windows.Controls;

namespace HotelRezervacije
{

    public partial class KarticaImenaKorisnik : UserControl
    {
        public string ImeGosta { get; set; }
        public string PrezimeGosta { get; set; }

        public KarticaImenaKorisnik()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}