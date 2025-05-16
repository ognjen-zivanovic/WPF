using System.Configuration;
using System.Data;
using System.Windows;

namespace HotelRezervacije
{
    public partial class App : Application
    {
        public App()
        {
            DatabaseManager.Init();
        }
    }
}
