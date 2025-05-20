using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;


namespace HotelRezervacije
{
    public static class MenadzerResursa
    {
        public static string IkonicaPromeniSliku = "slike/ikonica_promeni_sliku.jpg";
        public static string NemaSlike = "slike/nema_slike.jpg";
        public static FontFamily UnikodFont = (FontFamily)Application.Current.Resources["UnikodFont"];

        public static ImageSource IzvorOdImenaFajla(string imeFajla)
        {
            if (File.Exists(imeFajla))
            {
                byte[] bajtovi = File.ReadAllBytes(imeFajla);
                return IzvorOdNizaBajtova(bajtovi);
            }
            return null;
        }

        public static ImageSource IzvorOdNizaBajtova(byte[] nizBajtova)
        {
            if (nizBajtova != null)
            {
                using (var ms = new MemoryStream(nizBajtova))
                {
                    BitmapImage bitmapSlika = new BitmapImage();
                    bitmapSlika.BeginInit();
                    bitmapSlika.StreamSource = ms;
                    bitmapSlika.EndInit();
                    return bitmapSlika;
                }
            }
            return null;
        }
    }
}