using System.Windows.Media;
using System.Data.SQLite;
using System.IO;
using Dapper;

namespace HotelRezervacije
{
    public static class MenadzerBazePodataka
    {
        const string DbPath = "hotel.db";
        static string KonekcijaTekst => $"Data Source={DbPath};Version=3;";
        private static SQLiteConnection konekcija;

        public static void Pokreni()
        {
            MapiranjeBazePodataka.PodesiMapiranja();

            if (!File.Exists(DbPath))
            {
                SQLiteConnection.CreateFile(DbPath);
                konekcija = new SQLiteConnection(KonekcijaTekst);
                konekcija.Open();
                KreirajBazuPodataka();
            }
            else
            {
                konekcija = new SQLiteConnection(KonekcijaTekst);
                konekcija.Open();
            }
        }

        static void KreirajBazuPodataka()
        {
            KreirajTabele();
            DodajPrimerPodatke();
            DodajPrimerSlike();
        }

        static void KreirajTabele()
        {
            string krirajTabele = @"
CREATE TABLE sobe (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    ime TEXT,
    kapacitet INTEGER NOT NULL,
    cena_po_noci DECIMAL(10, 2),
    opis TEXT
);

CREATE TABLE korisnici (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    ime TEXT NOT NULL,
    prezime TEXT NOT NULL,
    email TEXT NOT NULL UNIQUE,
    telefon TEXT
);

CREATE TABLE rezervacije (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    soba_id INTEGER NOT NULL,
    korisnik_id INTEGER NOT NULL,
    datum_dolaska DATE NOT NULL,
    datum_odlaska DATE NOT NULL,
    broj_gostiju INTEGER NOT NULL,
    ukupna_cena DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (soba_id) REFERENCES sobe(id),
    FOREIGN KEY (korisnik_id) REFERENCES korisnici(id)
);

CREATE TABLE slike (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    soba_id INTEGER,
    slika_podaci BLOB,
    FOREIGN KEY (soba_id) REFERENCES sobe(id)
);

CREATE TABLE pogodnosti (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    ime TEXT NOT NULL,
    ikonica TEXT
);

CREATE TABLE soba_pogodnosti (
    soba_id INTEGER NOT NULL,
    pogodnost_id INTEGER NOT NULL,
    PRIMARY KEY (soba_id, pogodnost_id),
    FOREIGN KEY (soba_id) REFERENCES sobe(id),
    FOREIGN KEY (pogodnost_id) REFERENCES pogodnosti(id)
);

CREATE TABLE gosti (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    ime TEXT NOT NULL,
    prezime TEXT NOT NULL
);

CREATE TABLE gost_rezervacije (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    rezervacija_id INTEGER NOT NULL,
    gost_id INTEGER NOT NULL,
    FOREIGN KEY (rezervacija_id) REFERENCES rezervacije(id),
    FOREIGN KEY (gost_id) REFERENCES gosti(id)
);";
            konekcija.Execute(krirajTabele);
        }
        public static void DodajSliku(int sobaId, string imeFajlaSlike)
        {
            if (File.Exists(imeFajlaSlike))
            {
                byte[] bajtoviSlike = File.ReadAllBytes(imeFajlaSlike);
                konekcija.Execute(
                    "INSERT INTO slike (soba_id, slika_podaci) VALUES (@sobaId, @slikaPodaci)",
                    new { sobaId, slikaPodaci = bajtoviSlike }
                );
            }
        }

        public static ImageSource UcitajSlikuIzBazePodataka(int sobaId)
        {
            var imageBytes = konekcija.QueryFirstOrDefault<byte[]>(
                "SELECT slika_podaci FROM slike WHERE soba_id = @sobaId",
                new { sobaId }
            );
            return MenadzerResursa.IzvorOdNizaBajtova(imageBytes);
        }

        public static Soba[] UcitajSveSobe()
        {
            return konekcija.Query<Soba>("SELECT * FROM sobe").ToArray();
        }

        public static Soba[] UcitajOdgovarajuceSobe(DateTime datumDolaska, DateTime datumOdlaska, int ukupnoGostiju, string[] pogodnosti)
        {
            var query = @"
                SELECT * 
                FROM sobe 
                WHERE id NOT IN (
                    SELECT soba_id FROM rezervacije WHERE 
                    (datum_dolaska >= @datumDolaska AND datum_dolaska <= @datumOdlaska) OR
                    (datum_odlaska >= @datumDolaska AND datum_odlaska <= @datumOdlaska)
                ) 
                AND kapacitet >= @ukupnoGostiju";

            if (pogodnosti.Length > 0)
            {
                query += @"
                    AND id IN (
                        SELECT soba_id
                        FROM soba_pogodnosti sp JOIN pogodnosti p ON sp.pogodnost_id = p.id
                        WHERE p.ime IN @pogodnosti
                        GROUP BY soba_id
                        HAVING COUNT(DISTINCT p.ime) = @brojPogodnosti
                    )";
            }

            return konekcija.Query<Soba>(query, new
            {
                datumDolaska,
                datumOdlaska,
                ukupnoGostiju,
                pogodnosti,
                brojPogodnosti = pogodnosti.Length
            }).ToArray();
        }

        public static Soba UcitajSobuSaId(int sobaId)
        {
            return konekcija.QueryFirstOrDefault<Soba>(
                "SELECT * FROM sobe WHERE id = @sobaId",
                new { sobaId = sobaId }
            );
        }

        public static int DodajSobu(Soba room)
        {
            return konekcija.QuerySingle<int>(@"
                INSERT INTO sobe (ime, kapacitet, cena_po_noci, opis) 
                VALUES (@Ime, @Kapacitet, @CenaPoNoci, @Opis);
                SELECT last_insert_rowid();",
                room);
        }

        public static void IzmeniSobu(int sobaId, string ime, int kapacitet, decimal cenaPoNoci, string opis)
        {
            konekcija.Execute(@"
                UPDATE sobe 
                SET ime = @Ime, kapacitet = @Kapacitet, cena_po_noci = @Cena, opis = @Opis 
                WHERE id = @Id",
                new { Id = sobaId, Ime = ime, Kapacitet = kapacitet, Cena = cenaPoNoci, Opis = opis });
        }

        public static void ObrisiSobu(int sobaId)
        {
            konekcija.Execute("DELETE FROM sobe WHERE id = @sobaId", new { sobaId = sobaId });
        }

        public static Pogodnost[] UcitajSvePogodnosti()
        {
            return konekcija.Query<Pogodnost>("SELECT * FROM pogodnosti").ToArray();
        }

        public static Pogodnost[] UcitajPogodnostiZaSobu(int sobaId)
        {
            return konekcija.Query<Pogodnost>(@"
                SELECT p.* 
                FROM pogodnosti p
                JOIN soba_pogodnosti sp ON p.id = sp.pogodnost_id
                WHERE sp.soba_id = @sobaId",
                new { sobaId = sobaId }
            ).ToArray();
        }



        public static void ObrisiPogodnostIzSobe(int sobaId, int pogodnostId)
        {
            konekcija.Execute(
                "DELETE FROM soba_pogodnosti WHERE soba_id = @sobaId AND pogodnost_id = @PogodnostId",
                new { sobaId = sobaId, PogodnostId = pogodnostId }
            );
        }

        public static void ObrisiSvePogodnostiIzSobe(int sobaId)
        {
            konekcija.Execute(
                "DELETE FROM soba_pogodnosti WHERE soba_id = @sobaId",
                new { sobaId = sobaId }
            );
        }

        public static void DodajPogodnostSobi(int sobaId, int pogodnostId)
        {
            konekcija.Execute(
                "INSERT INTO soba_pogodnosti (soba_id, pogodnost_id) VALUES (@sobaId, @PogodnostId)",
                new { sobaId = sobaId, PogodnostId = pogodnostId }
            );
        }

        public static void DodajPogodnost(Pogodnost amenity)
        {
            konekcija.Execute(
                "INSERT INTO pogodnosti (ime, ikonica) VALUES (@Ime, @Ikonica)",
                amenity
            );
        }

        public static void ObrisiPogodnost(int pogodnostId)
        {
            konekcija.Execute(
                "DELETE FROM pogodnosti WHERE id = @PogodnostId",
                new { PogodnostId = pogodnostId }
            );
        }

        public static void IzmeniPogodnost(int pogodnostId, string ime, string ikonica)
        {
            konekcija.Execute(
                "UPDATE pogodnosti SET ime = @Ime, ikonica = @Ikonica WHERE id = @PogodnostId",
                new { PogodnostId = pogodnostId, Ime = ime, Ikonica = ikonica }
            );
        }

        public static Soba[] UcitajSobePoIdPogodnosti(int pogodnostId)
        {
            return konekcija.Query<Soba>(@"
                SELECT s.* 
                FROM sobe s
                JOIN soba_pogodnosti sp ON s.id = sp.soba_id
                WHERE sp.pogodnost_id = @PogodnostId",
                new { PogodnostId = pogodnostId }
            ).ToArray();
        }

        public static int DodajRezervaciju(Rezervacija rezervacija)
        {
            return konekcija.QuerySingle<int>(@"
                INSERT INTO rezervacije (soba_id, korisnik_id, datum_dolaska, datum_odlaska, broj_gostiju, ukupna_cena)
                VALUES (@SobaId, @KorisnikId, @DatumDolaska, @DatumOdlaska, @BrojGostiju, @UkupnaCena);
                SELECT last_insert_rowid();",
                rezervacija);
        }

        public static int DodajKorisnika(Korisnik korisnik)
        {
            return konekcija.QuerySingle<int>(@"
                INSERT INTO korisnici (ime, prezime, email, telefon)
                VALUES (@Ime, @Prezime, @Email, @Telefon);
                SELECT last_insert_rowid();",
                korisnik);
        }

        public static int DodajGosta(Gost gost)
        {
            return konekcija.QuerySingle<int>(@"
                INSERT INTO gosti (ime, prezime)
                VALUES (@Ime, @Prezime);
                SELECT last_insert_rowid();",
                gost);
        }

        public static void DodajGostRezervaciju(int rezervacijaId, int guestId)
        {
            konekcija.Execute(
                "INSERT INTO gost_rezervacije (rezervacija_id, gost_id) VALUES (@RezervacijaId, @GuestId)",
                new { RezervacijaId = rezervacijaId, GuestId = guestId }
            );
        }

        public static KarticaRezervacije[] UcitajPretrazeneRezervacije(string tekstPretrage)
        {
            var upit = @"
                SELECT r.*, k.*, s.*
                FROM rezervacije r
                JOIN korisnici k ON r.korisnik_id = k.id
                JOIN sobe s ON r.soba_id = s.id";

            if (!string.IsNullOrEmpty(tekstPretrage))
            {
                var terminiPretrage = tekstPretrage.Split(' ').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s));
                var usloviPretrage = terminiPretrage.Select(term =>
                    $"(k.ime LIKE '%{term}%' OR k.prezime LIKE '%{term}%' OR s.ime LIKE '%{term}%' OR k.email LIKE '%{term}%' OR k.telefon LIKE '%{term}%')"
                );
                upit += " WHERE " + string.Join(" AND ", usloviPretrage);
            }
            upit += " ORDER BY r.datum_dolaska ASC";

            return konekcija.Query<Rezervacija, Korisnik, Soba, KarticaRezervacije>(
                upit,
                (rezervacija, korisnik, soba) => new KarticaRezervacije
                {
                    Rezervacija = rezervacija,
                    Korisnik = korisnik,
                    Soba = soba
                },
                splitOn: "id"
            ).ToArray();
        }

        public static Gost[] UcitajGostePoIdRezervacije(int rezervacijaId)
        {
            return konekcija.Query<Gost>(@"
                SELECT g.* 
                FROM gosti g
                JOIN gost_rezervacije gr ON g.id = gr.gost_id
                WHERE gr.rezervacija_id = @RezervacijaId",
                new { RezervacijaId = rezervacijaId }
            ).ToArray();
        }

        public static Rezervacija[] UcitajRezervacijeZaSobu(int sobaId)
        {
            return konekcija.Query<Rezervacija>(
                "SELECT * FROM rezervacije WHERE soba_id = @sobaId",
                new { sobaId = sobaId }
            ).ToArray();
        }

        public static void DodajSliku(int sobaId, byte[] imageBytes)
        {
            konekcija.Execute(
                "INSERT INTO slike (soba_id, slika_podaci) VALUES (@sobaId, @slikaPodaci)",
                new { sobaId = sobaId, slikaPodaci = imageBytes }
            );
        }

        public static void PromeniSlikuSobe(int sobaId, byte[] imageBytes)
        {
            konekcija.Execute(
                "UPDATE slike SET slika_podaci = @slikaPodaci WHERE soba_id = @sobaId",
                new { sobaId = sobaId, slikaPodaci = imageBytes }
            );
        }

        public static void ObrisiSveSobeSaIdSobe(int sobaId)
        {
            konekcija.Execute(
                "DELETE FROM slike WHERE soba_id = @sobaId",
                new { sobaId = sobaId }
            );
        }

        static void DodajPrimerPodatke()
        {
            string dodajSobe = @"
 INSERT INTO sobe (ime, kapacitet, cena_po_noci, opis) VALUES
 ('Jednokrevetna Oaza', 1, 99.99, '1 francuski ležaj, TV, privatno kupatilo, 20m²'),
 ('Komforna Dvokrevetna', 2, 149.99, '2 odvojena kreveta, TV, mini frižider, 25m²'),
 ('Porodična Soba', 4, 299.99, '2 bračna kreveta, TV, čajna kuhinja, 40m²'),
 ('Luksuzni Porodični Apartman', 5, 499.99, '2 bračna kreveta + trosed na razvlačenje, dnevni boravak, TV, 55m²'),
 ('Prostrana Porodična Soba', 4, 249.99, '1 king size krevet + 2 kreveta za jednu osobu, radni sto, TV, 35m²'),
 ('Romantični Apartman', 2, 349.99, '1 king size krevet, TV, đakuzi, 30m²'),
 ('Moderna Dvokrevetna', 2, 119.99, '2 kreveta za jednu osobu, TV, radni kutak, 22m²'),
 ('Ekonomična Jednokrevetna', 1, 79.99, '1 krevet za jednu osobu, manji TV, 18m²'),
 ('Trokrevetna Komfor Soba', 3, 399.99, '1 bračni krevet + 1 jednokrevetni, TV, fotelja, 32m²'),
 ('Predsednički Apartman', 6, 699.99, '3 bračna kreveta, veliki dnevni boravak, 2 TV-a, kuhinja, 70m²');
";

            string dodajKorisnike = @"
 INSERT INTO korisnici(ime, prezime, email, telefon) VALUES
 ('Marko', 'Petrović', 'marko.petrovic@example.com', '0641234567'),
 ('Jelena', 'Jovanović', 'jelena.jovanovic@example.com', '0652345678'),
 ('Nikola', 'Nikolić', 'nikola.nikolic@example.com', '0663456789'),
 ('Ana', 'Stanković', 'ana.stankovic@example.com', '0674567890'),
 ('Miloš', 'Đorđević', 'milos.djordjevic@example.com', '0685678901'),
 ('Ivana', 'Milenković', 'ivana.milenkovic@example.com', '0696789012'),
 ('Stefan', 'Lazarević', 'stefan.lazarevic@example.com', '0607890123'),
 ('Marija', 'Ilić', 'marija.ilic@example.com', '0618901234'),
 ('Vuk', 'Simić', 'vuk.simic@example.com', '0629012345'),
 ('Teodora', 'Marković', 'teodora.markovic@example.com', '0630123456'),
 ('Lazar', 'Ristić', 'lazar.ristic@example.com', '0641112222'),
 ('Milica', 'Todorović', 'milica.todorovic@example.com', '0652223333'),
 ('Aleksandar', 'Kovačević', 'aleksandar.kovacevic@example.com', '0663334444'),
 ('Katarina', 'Matić', 'katarina.matic@example.com', '0674445555'),
 ('Nemanja', 'Pavlović', 'nemanja.pavlovic@example.com', '0685556666'),
 ('Sara', 'Janković', 'sara.jankovic@example.com', '0696667777'),
 ('Boško', 'Milutinović', 'bosko.milutinovic@example.com', '0607778888'),
 ('Jovana', 'Obradović', 'jovana.obradovic@example.com', '0618889999'),
 ('Uroš', 'Stevanović', 'uros.stevanovic@example.com', '0629990000'),
 ('Nevena', 'Gajić', 'nevena.gajic@example.com', '0630001111'),
 ('Filip', 'Vučić', 'filip.vucic@example.com', '0641012020'),
 ('Tamara', 'Živković', 'tamara.zivkovic@example.com', '0652023030'),
 ('Dragan', 'Rakić', 'dragan.rakic@example.com', '0663034040'),
 ('Milena', 'Radović', 'milena.radovic@example.com', '0674045050'),
 ('Petar', 'Lukić', 'petar.lukic@example.com', '0685056060'),
 ('Isidora', 'Vasić', 'isidora.vasic@example.com', '0696067070'),
 ('Žarko', 'Bogdanović', 'zarko.bogdanovic@example.com', '0607078080'),
 ('Nataša', 'Tasić', 'natasa.tasic@example.com', '0618089090'),
 ('Ilija', 'Mirković', 'ilija.mirkovic@example.com', '0629090101'),
 ('Vesna', 'Šarić', 'vesna.saric@example.com', '0630101112'),
 ('Ognjen', 'Stanojević', 'ognjen.stanojevic@example.com', '0641112122'),
 ('Tatjana', 'Grujić', 'tatjana.grujic@example.com', '0652123132'),
 ('Andrija', 'Milojević', 'andrija.milojevic@example.com', '0663134142');
";

            string dodajRezervacije = @"
 INSERT INTO rezervacije (soba_id, korisnik_id, datum_dolaska, datum_odlaska, broj_gostiju, ukupna_cena) VALUES
 (1, 1, '2025-05-10', '2025-05-12', 1, 599.99),
 (2, 2, '2025-05-15', '2025-05-18', 2, 576.28),
 (3, 5, '2025-06-01', '2025-06-05', 2, 820.00),
 (4, 3, '2025-05-20', '2025-05-22', 1, 460.50),
 (5, 6, '2025-06-10', '2025-06-12', 3, 1015.75),
 (2, 7, '2025-07-01', '2025-07-04', 2, 712.00),
 (1, 8, '2025-08-15', '2025-08-18', 1, 580.20),
 (3, 9, '2025-09-05', '2025-09-08', 2, 775.90),
 (4, 10, '2025-05-22', '2025-05-25', 1, 689.99),
 (5, 11, '2025-06-15', '2025-06-17', 2, 640.00),
 (1, 12, '2025-07-10', '2025-07-13', 2, 845.60),
 (2, 13, '2025-07-20', '2025-07-22', 1, 399.99),
 (3, 14, '2025-08-01', '2025-08-03', 2, 920.10),
 (4, 15, '2025-08-10', '2025-08-12', 3, 999.99),
 (5, 16, '2025-09-01', '2025-09-03', 2, 570.75),
 (2, 17, '2025-05-25', '2025-05-27', 1, 455.35),
 (1, 18, '2025-06-20', '2025-06-22', 2, 600.00),
 (3, 19, '2025-06-25', '2025-06-27', 2, 890.90),
 (4, 20, '2025-07-15', '2025-07-18', 1, 705.00),
 (5, 21, '2025-07-25', '2025-07-28', 2, 820.50),
 (1, 22, '2025-08-05', '2025-08-07', 1, 500.00),
 (2, 23, '2025-09-10', '2025-09-13', 2, 845.30),
 (3, 24, '2025-05-05', '2025-05-07', 1, 430.40),
 (4, 25, '2025-06-01', '2025-06-03', 3, 980.25),
 (5, 26, '2025-06-05', '2025-06-08', 2, 1020.60),
 (1, 27, '2025-07-03', '2025-07-05', 2, 765.00),
 (2, 28, '2025-08-08', '2025-08-10', 1, 495.20),
 (3, 29, '2025-08-20', '2025-08-23', 2, 740.99),
 (4, 30, '2025-09-15', '2025-09-18', 2, 860.80),
 (5, 31, '2025-05-18', '2025-05-20', 1, 555.50),
 (1, 32, '2025-06-18', '2025-06-21', 3, 1120.00),
 (2, 33, '2025-07-28', '2025-07-30', 2, 675.75),
 (3, 34, '2025-08-25', '2025-08-28', 1, 599.95);";

            string dodajPogodnosti = @"
 INSERT INTO pogodnosti (ime, ikonica) VALUES
 ('WiFi internet',       CHAR(0xf1eb)),  -- fa-wifi
 ('Privatno kupatilo',   CHAR(0xf2cd)),  -- fa-bath
 ('Klima uređaj',        CHAR(0xf2c9)),  -- fa-snowflake
 ('Grejanje',            CHAR(0xf06d)),  -- fa-fire
 ('Televizor',           CHAR(0xf26c)),  -- fa-tv
 ('Toaletni pribor',     CHAR(0xf62e)),  -- fa-pump-medical
 ('Fen za kosu',         CHAR(0xf72e)),  -- fa-wind (zamena)
 ('Frižider',            CHAR(0xf2c9)),  -- fa-snowflake (simbolično)
 ('Aparat za kafu',      CHAR(0xf0f4));  -- fa-coffee
";

            string dodajPogodnostiSobe = @"
 INSERT INTO soba_pogodnosti (soba_id, pogodnost_id) VALUES
 (1, 1), (1, 2), (1, 3),
 (2, 1), (2, 3), (2, 5), (2, 6),
 (3, 1), (3, 2), (3, 4), (3, 7),
 (4, 1), (4, 3), (4, 5), (4, 9),
 (5, 1), (5, 2), (5, 6), (5, 7),
 (6, 1), (6, 4), (6, 5),
 (7, 1), (7, 3),
 (8, 1), (8, 2), (8, 6),
 (9, 1), (9, 4), (9, 5),
 (10, 1), (10, 3), (10, 8);
 ";



            konekcija.Execute(dodajSobe);
            konekcija.Execute(dodajKorisnike);
            konekcija.Execute(dodajRezervacije);
            konekcija.Execute(dodajPogodnostiSobe);
            konekcija.Execute(dodajPogodnosti);
        }

        static void DodajPrimerSlike()
        {
            for (int i = 1; i <= 10; i++)
            {
                DodajSliku(i, "slike/slika_sobe.jpg");
            }
        }
    }
}