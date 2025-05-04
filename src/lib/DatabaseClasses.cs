namespace WpfApp1
{

    //    return @"
    //             CREATE TABLE rooms (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT, capacity INTEGER NOT NULL, price_per_night DECIMAL(10, 2));
    //             CREATE TABLE users (id INTEGER PRIMARY KEY AUTOINCREMENT, full_name TEXT NOT NULL, email TEXT NOT NULL UNIQUE, phone TEXT);
    //             CREATE TABLE reservations (id INTEGER PRIMARY KEY AUTOINCREMENT, room_id INTEGER NOT NULL, user_id INTEGER NOT NULL, check_in DATE NOT NULL, check_out DATE NOT NULL, adults INTEGER NOT NULL, children INTEGER NOT NULL, FOREIGN KEY (room_id) REFERENCES rooms(id), FOREIGN KEY (user_id) REFERENCES users(id));
    //             CREATE TABLE images (id INTEGER PRIMARY KEY AUTOINCREMENT, room_id INTEGER, image_data BLOB, FOREIGN KEY (room_id) REFERENCES rooms(id));";


    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public float PricePerNight { get; set; }
        public string Description { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class Guest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }

    public class GuestReservations
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int GuestId { get; set; }
    }
    public class Reservation
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
    }
    public class DatabaseImage
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public byte[] ImageData { get; set; }
    }

    public class Amenity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Icon { get; set; }
    }
}