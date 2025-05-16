
using Dapper;
using System;
using System.Reflection;

namespace HotelRezervacije
{
    public static class DatabaseMapping
    {
        public static void PodesiMapiranja()
        {
            // Room (Soba)
            SqlMapper.SetTypeMap(typeof(Soba), new CustomPropertyTypeMap(
                typeof(Soba),
                (type, column) => column switch
                {
                    "id" => type.GetProperty("Id"),
                    "ime" => type.GetProperty("Ime"),
                    "kapacitet" => type.GetProperty("Kapacitet"),
                    "cena_po_noci" => type.GetProperty("CenaPoNoci"),
                    "opis" => type.GetProperty("Opis"),
                    _ => null
                }));

            // User (Korisnik)
            SqlMapper.SetTypeMap(typeof(Korisnik), new CustomPropertyTypeMap(
                typeof(Korisnik),
                (type, column) => column switch
                {
                    "id" => type.GetProperty("Id"),
                    "ime" => type.GetProperty("Ime"),
                    "prezime" => type.GetProperty("Prezime"),
                    "email" => type.GetProperty("Email"),
                    "telefon" => type.GetProperty("Telefon"),
                    _ => null
                }));

            // Reservation (Rezervacija)
            SqlMapper.SetTypeMap(typeof(Rezervacija), new CustomPropertyTypeMap(
                typeof(Rezervacija),
                (type, column) => column switch
                {
                    "id" => type.GetProperty("Id"),
                    "soba_id" => type.GetProperty("SobaId"),
                    "korisnik_id" => type.GetProperty("KorisnikId"),
                    "check_in" => type.GetProperty("CheckIn"),
                    "check_out" => type.GetProperty("CheckOut"),
                    "broj_gostiju" => type.GetProperty("BrojGostiju"),
                    "ukupna_cena" => type.GetProperty("UkupnaCena"),
                    _ => null
                }));

            // Image (Slika)
            SqlMapper.SetTypeMap(typeof(Slika), new CustomPropertyTypeMap(
                typeof(Slika),
                (type, column) => column switch
                {
                    "id" => type.GetProperty("Id"),
                    "soba_id" => type.GetProperty("SobaId"),
                    "slika_podaci" => type.GetProperty("slikaPodaci"),
                    _ => null
                }));

            // Amenity (Pogodnost)
            SqlMapper.SetTypeMap(typeof(Pogodnost), new CustomPropertyTypeMap(
                typeof(Pogodnost),
                (type, column) => column switch
                {
                    "id" => type.GetProperty("Id"),
                    "ime" => type.GetProperty("Ime"),
                    "ikonica" => type.GetProperty("Ikonica"),
                    _ => null
                }));

            // Guest (Gost)
            SqlMapper.SetTypeMap(typeof(Gost), new CustomPropertyTypeMap(
                typeof(Gost),
                (type, column) => column switch
                {
                    "id" => type.GetProperty("Id"),
                    "ime" => type.GetProperty("Ime"),
                    "prezime" => type.GetProperty("Prezime"),
                    _ => null
                }));

            // GuestReservation (GostRezervacije)
            SqlMapper.SetTypeMap(typeof(GostRezervacije), new CustomPropertyTypeMap(
                typeof(GostRezervacije),
                (type, column) => column switch
                {
                    "id" => type.GetProperty("Id"),
                    "rezervacija_id" => type.GetProperty("RezervacijaId"),
                    "gost_id" => type.GetProperty("GostId"),
                    _ => null
                }));
        }
    }
}
