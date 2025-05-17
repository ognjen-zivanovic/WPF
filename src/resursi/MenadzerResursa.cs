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
using Dapper;
using System.Linq;
using System.Collections.Generic;


namespace HotelRezervacije
{
    public static class MenadzerResursa
    {
        public static string IkonicaPromeniSliku = "slike/ikonica_promeni_sliku.jpg";
        public static string NemaSlike = "slike/nema_slike.jpg";
        public static FontFamily UnikodFont = (FontFamily)Application.Current.Resources["UnicodeFont"];

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

        public static ImageSource IzvorOdImenaFajla(string imeFajla)
        {
            if (File.Exists(imeFajla))
            {
                using (var fs = new FileStream(imeFajla, FileMode.Open, FileAccess.Read))
                {
                    BitmapImage bitmapSlika = new BitmapImage();
                    bitmapSlika.BeginInit();
                    bitmapSlika.StreamSource = fs;
                    bitmapSlika.EndInit();
                    return bitmapSlika;
                }
            }
            return null;
        }
    }
}