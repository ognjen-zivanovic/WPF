namespace HotelRezervacije
{
    public class Soba
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public int Kapacitet { get; set; }
        public decimal CenaPoNoci { get; set; }
        public string Opis { get; set; }
    }

    public class Korisnik
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
    }

    public class Gost
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
    }

    public class GostRezervacije
    {
        public int Id { get; set; }
        public int RezervacijaId { get; set; }
        public int GostId { get; set; }
    }

    public class Rezervacija
    {
        public int Id { get; set; }
        public int SobaId { get; set; }
        public int KorisnikId { get; set; }
        public DateTime DatumDolaska { get; set; }
        public DateTime DatumOdlaska { get; set; }
        public int BrojGostiju { get; set; }
        public decimal UkupnaCena { get; set; }
    }

    public class Slika
    {
        public int Id { get; set; }
        public int SobaId { get; set; }
        public byte[] SlikaPodaci { get; set; }
    }

    public class Pogodnost
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Ikonica { get; set; }
    }
}