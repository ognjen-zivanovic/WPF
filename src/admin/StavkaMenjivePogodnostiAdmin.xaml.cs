using System.Windows.Controls;

namespace HotelRezervacije
{
    public partial class StavkaMenjivePogodnostiAdmin : UserControl
    {
        Pogodnost Pogodnost;
        public StavkaMenjivePogodnostiAdmin(int id, string ime, string ikonica)
        {
            InitializeComponent();
            Pogodnost = new Pogodnost
            {
                Id = id,
                Ime = ime,
                Ikonica = ikonica
            };

            IkonicaTextBox.Text = ikonica;
            ImeTextBox.Text = ime;
        }
        private void IkonicaTextBox_TextChanged(object s, TextChangedEventArgs e)
        {
            Pogodnost.Ikonica = ((TextBox)s).Text;
            MenadzerBazePodataka.IzmeniPogodnost(Pogodnost.Id, Pogodnost.Ime, Pogodnost.Ikonica);
        }
        private void ImeTextBox_TextChanged(object s, TextChangedEventArgs e)
        {
            Pogodnost.Ime = ((TextBox)s).Text;
            MenadzerBazePodataka.IzmeniPogodnost(Pogodnost.Id, Pogodnost.Ime, Pogodnost.Ikonica);
        }
    }
}
