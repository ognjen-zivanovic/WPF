using System.Windows;

namespace HotelRezervacije
{
    public partial class App : Application
    {
        public App()
        {
            MenadzerBazePodataka.Pokreni();
        }
    }
}
