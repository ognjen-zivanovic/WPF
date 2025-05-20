
using Dapper;

namespace HotelRezervacije
{
    public static class MapiranjeBazePodataka
    {
        public static void PodesiMapiranja()
        {
            SqlMapper.SetTypeMap(typeof(Soba), new CustomPropertyTypeMap(
                typeof(Soba),
                (tipKlase, kolona) => kolona switch
                {
                    "id" => tipKlase.GetProperty("Id"),
                    "ime" => tipKlase.GetProperty("Ime"),
                    "kapacitet" => tipKlase.GetProperty("Kapacitet"),
                    "cena_po_noci" => tipKlase.GetProperty("CenaPoNoci"),
                    "opis" => tipKlase.GetProperty("Opis"),
                    _ => null
                }));

            SqlMapper.SetTypeMap(typeof(Korisnik), new CustomPropertyTypeMap(
                typeof(Korisnik),
                (tipKlase, kolona) => kolona switch
                {
                    "id" => tipKlase.GetProperty("Id"),
                    "ime" => tipKlase.GetProperty("Ime"),
                    "prezime" => tipKlase.GetProperty("Prezime"),
                    "email" => tipKlase.GetProperty("Email"),
                    "telefon" => tipKlase.GetProperty("Telefon"),
                    _ => null
                }));

            SqlMapper.SetTypeMap(typeof(Rezervacija), new CustomPropertyTypeMap(
                typeof(Rezervacija),
                (tipKlase, kolona) => kolona switch
                {
                    "id" => tipKlase.GetProperty("Id"),
                    "soba_id" => tipKlase.GetProperty("SobaId"),
                    "korisnik_id" => tipKlase.GetProperty("KorisnikId"),
                    "datum_dolaska" => tipKlase.GetProperty("DatumDolaska"),
                    "datum_odlaska" => tipKlase.GetProperty("DatumOdlaska"),
                    "broj_gostiju" => tipKlase.GetProperty("BrojGostiju"),
                    "ukupna_cena" => tipKlase.GetProperty("UkupnaCena"),
                    _ => null
                }));

            SqlMapper.SetTypeMap(typeof(Slika), new CustomPropertyTypeMap(
                typeof(Slika),
                (tipKlase, kolona) => kolona switch
                {
                    "id" => tipKlase.GetProperty("Id"),
                    "soba_id" => tipKlase.GetProperty("SobaId"),
                    "slika_podaci" => tipKlase.GetProperty("slikaPodaci"),
                    _ => null
                }));

            SqlMapper.SetTypeMap(typeof(Pogodnost), new CustomPropertyTypeMap(
                typeof(Pogodnost),
                (tipKlase, kolona) => kolona switch
                {
                    "id" => tipKlase.GetProperty("Id"),
                    "ime" => tipKlase.GetProperty("Ime"),
                    "ikonica" => tipKlase.GetProperty("Ikonica"),
                    _ => null
                }));

            SqlMapper.SetTypeMap(typeof(Gost), new CustomPropertyTypeMap(
                typeof(Gost),
                (tipKlase, kolona) => kolona switch
                {
                    "id" => tipKlase.GetProperty("Id"),
                    "ime" => tipKlase.GetProperty("Ime"),
                    "prezime" => tipKlase.GetProperty("Prezime"),
                    _ => null
                }));

            SqlMapper.SetTypeMap(typeof(GostRezervacije), new CustomPropertyTypeMap(
                typeof(GostRezervacije),
                (tipKlase, kolona) => kolona switch
                {
                    "id" => tipKlase.GetProperty("Id"),
                    "rezervacija_id" => tipKlase.GetProperty("RezervacijaId"),
                    "gost_id" => tipKlase.GetProperty("GostId"),
                    _ => null
                }));
        }
    }
}
