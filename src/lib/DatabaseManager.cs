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

namespace WpfApp1
{
    public static class DatabaseManager
    {
        const string DbPath = "hotel.db";
        static string ConnectionString => $"Data Source={DbPath};Version=3;";

        public static void Init()
        {
            if (!File.Exists(DbPath))
            {
                CreateDatabase();
            }
        }

        static void CreateDatabase()
        {
            SQLiteConnection.CreateFile(DbPath);
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                var createTables = GetCreateTableQueries();
                using (var cmd = new SQLiteCommand(createTables, connection))
                {
                    cmd.ExecuteNonQuery();
                }
                InsertSampleData(connection);
            }
        }

        static string GetCreateTableQueries()
        {
            return @"
CREATE TABLE rooms (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT,
    capacity INTEGER NOT NULL,
    price_per_night DECIMAL(10, 2),
    description TEXT
);

CREATE TABLE users (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    surname TEXT NOT NULL,
    email TEXT NOT NULL UNIQUE,
    phone TEXT
);

CREATE TABLE reservations (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    room_id INTEGER NOT NULL,
    user_id INTEGER NOT NULL,
    check_in DATE NOT NULL,
    check_out DATE NOT NULL,
    adults INTEGER NOT NULL,
    children INTEGER NOT NULL,
    total_price DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (room_id) REFERENCES rooms(id),
    FOREIGN KEY (user_id) REFERENCES users(id)
);

CREATE TABLE images (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    room_id INTEGER,
    image_data BLOB,
    FOREIGN KEY (room_id) REFERENCES rooms(id)
);

CREATE TABLE amenities (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    icon TEXT
);

CREATE TABLE room_amenities (
    room_id INTEGER NOT NULL,
    amenity_id INTEGER NOT NULL,
    PRIMARY KEY (room_id, amenity_id),
    FOREIGN KEY (room_id) REFERENCES rooms(id),
    FOREIGN KEY (amenity_id) REFERENCES amenities(id)
);

CREATE TABLE guests (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    surname TEXT NOT NULL
);

CREATE TABLE guest_reservations (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    reservation_id INTEGER NOT NULL,
    guest_id INTEGER NOT NULL,
    FOREIGN KEY (reservation_id) REFERENCES reservations(id),
    FOREIGN KEY (guest_id) REFERENCES guests(id)
);
INSERT INTO amenities (name, icon) VALUES
('WiFi',              CHAR(0xf1eb)),  -- fa-wifi
('Private Bathroom',  CHAR(0xf2cd)),  -- fa-bath
('Air Conditioning',  CHAR(0xf2c9)),  -- fa-snowflake
('Heating',           CHAR(0xf06d)),  -- fa-fire
('TV',                CHAR(0xf26c)),  -- fa-tv
('Toiletries',        CHAR(0xf62e)),  -- fa-pump-medical
('Hair Dryer',        CHAR(0xf72e)),  -- fa-wind (alternative for hair dryer)
('Refrigerator',      CHAR(0xf2c9)),  -- fa-snowflake (symbolic substitute)
('Coffee Maker',      CHAR(0xf0f4));  -- fa-coffee


";
            // ('Closet'),
            // ('Desk'),
            // ('Safe'),
            // ('Free Parking'),
            // ('Daily Housekeeping');
        }

        static void InsertSampleData(SQLiteConnection connection)
        {
            string insertRooms = @"
            INSERT INTO rooms (name, capacity, price_per_night, description) VALUES
('Cozy Single Retreat', 1, 99.99, '1 queen-sized bed, TV, private bathroom, 20 square meters'),
('Deluxe Double Comfort', 2, 149.99, '2 single beds, TV, mini fridge, 25 square meters'),
('Family Suite', 4, 299.99, '2 queen-sized beds, TV, kitchenette, 40 square meters'),
('Luxury Family Apartment', 5, 499.99, '2 queen-sized beds + 1 sofa bed, living area, TV, 55 square meters'),
('Executive Family Room', 4, 249.99, '1 king-sized bed + 2 single beds, work desk, TV, 35 square meters'),
('Romantic King Suite', 2, 349.99, '1 king-sized bed, TV, spa bath, 30 square meters'),
('Modern Twin Room', 2, 119.99, '2 single beds, TV, compact workspace, 22 square meters'),
('Budget Single Room', 1, 79.99, '1 single bed, small TV, 18 square meters'),
('Triple Comfort Room', 3, 399.99, '1 queen-sized bed + 1 single bed, TV, lounge chair, 32 square meters'),
('Presidential Suite', 6, 699.99, '3 queen-sized beds, large living area, 2 TVs, kitchen, 70 square meters');";


            string insertUsers = @"
    INSERT INTO users(name, surname, email, phone) VALUES
    ('John', 'Doe', 'john.doe@example.com', '123-456-7890'), 
    ('Jane', 'Smith', 'jane.smith@example.com', '987-654-3210'),
    ('Bob', 'Johnson', 'bob.johnson@example.com', '555-555-5555'),
    ('Alice', 'Brown', 'alice.brown@example.com', '234-567-8901'),
    ('Charlie', 'Davis', 'charlie.davis@example.com', '345-678-9012'),
    ('Emily', 'Evans', 'emily.evans@example.com', '456-789-0123'),
    ('Daniel', 'Wilson', 'daniel.wilson@example.com', '567-890-1234'),
    ('Fiona', 'Taylor', 'fiona.taylor@example.com', '678-901-2345'),
    ('George', 'Anderson', 'george.anderson@example.com', '789-012-3456'),
    ('Hannah', 'Thomas', 'hannah.thomas@example.com', '890-123-4567'),
    ('Ian', 'Martinez', 'ian.martinez@example.com', '901-234-5678'),
    ('Julia', 'Garcia', 'julia.garcia@example.com', '012-345-6789'),
    ('Kevin', 'Robinson', 'kevin.robinson@example.com', '111-222-3333'),
    ('Laura', 'Clark', 'laura.clark@example.com', '222-333-4444'),
    ('Mark', 'Rodriguez', 'mark.rodriguez@example.com', '333-444-5555'),
    ('Nina', 'Lewis', 'nina.lewis@example.com', '444-555-6666'),
    ('Oscar', 'Lee', 'oscar.lee@example.com', '555-666-7777'),
    ('Paula', 'Walker', 'paula.walker@example.com', '666-777-8888'),
    ('Quentin', 'Hall', 'quentin.hall@example.com', '777-888-9999'),
    ('Rachel', 'Allen', 'rachel.allen@example.com', '888-999-0000'),
    ('Steve', 'Young', 'steve.young@example.com', '999-000-1111'),
    ('Tina', 'Hernandez', 'tina.hernandez@example.com', '000-111-2222'),
    ('Umar', 'King', 'umar.king@example.com', '101-202-3030'),
    ('Vera', 'Wright', 'vera.wright@example.com', '202-303-4040'),
    ('Will', 'Lopez', 'will.lopez@example.com', '303-404-5050'),
    ('Xena', 'Hill', 'xena.hill@example.com', '404-505-6060'),
    ('Yuri', 'Scott', 'yuri.scott@example.com', '505-606-7070'),
    ('Zara', 'Green', 'zara.green@example.com', '606-707-8080'),
    ('Adam', 'Baker', 'adam.baker@example.com', '707-808-9090'),
    ('Bella', 'Nelson', 'bella.nelson@example.com', '808-909-0101'),
    ('Carl', 'Carter', 'carl.carter@example.com', '909-010-1112'),
    ('Diana', 'Mitchell', 'diana.mitchell@example.com', '010-111-2122'),
    ('Ethan', 'Perez', 'ethan.perez@example.com', '111-212-3132'),
    ('Grace', 'Roberts', 'grace.roberts@example.com', '212-313-4142');";


            string insertReservations = @"
    INSERT INTO reservations (room_id, user_id, check_in, check_out, adults, children, total_price) VALUES
    (1, 1, '2025-05-10', '2025-05-12', 1, 0, 599.99),
    (2, 2, '2025-05-15', '2025-05-18', 2, 1, 576.28),
    (3, 5, '2025-06-01', '2025-06-05', 2, 0, 820.00),
    (4, 3, '2025-05-20', '2025-05-22', 1, 1, 460.50),
    (5, 6, '2025-06-10', '2025-06-12', 3, 2, 1015.75),
    (2, 7, '2025-07-01', '2025-07-04', 2, 0, 712.00),
    (1, 8, '2025-08-15', '2025-08-18', 1, 0, 580.20),
    (3, 9, '2025-09-05', '2025-09-08', 2, 1, 775.90),
    (4, 10, '2025-05-22', '2025-05-25', 1, 2, 689.99),
    (5, 11, '2025-06-15', '2025-06-17', 2, 0, 640.00),
    (1, 12, '2025-07-10', '2025-07-13', 2, 1, 845.60),
    (2, 13, '2025-07-20', '2025-07-22', 1, 0, 399.99),
    (3, 14, '2025-08-01', '2025-08-03', 2, 2, 920.10),
    (4, 15, '2025-08-10', '2025-08-12', 3, 1, 999.99),
    (5, 16, '2025-09-01', '2025-09-03', 2, 0, 570.75),
    (2, 17, '2025-05-25', '2025-05-27', 1, 1, 455.35),
    (1, 18, '2025-06-20', '2025-06-22', 2, 0, 600.00),
    (3, 19, '2025-06-25', '2025-06-27', 2, 2, 890.90),
    (4, 20, '2025-07-15', '2025-07-18', 1, 0, 705.00),
    (5, 21, '2025-07-25', '2025-07-28', 2, 1, 820.50),
    (1, 22, '2025-08-05', '2025-08-07', 1, 0, 500.00),
    (2, 23, '2025-09-10', '2025-09-13', 2, 1, 845.30),
    (3, 24, '2025-05-05', '2025-05-07', 1, 1, 430.40),
    (4, 25, '2025-06-01', '2025-06-03', 3, 0, 980.25),
    (5, 26, '2025-06-05', '2025-06-08', 2, 2, 1020.60),
    (1, 27, '2025-07-03', '2025-07-05', 2, 1, 765.00),
    (2, 28, '2025-08-08', '2025-08-10', 1, 0, 495.20),
    (3, 29, '2025-08-20', '2025-08-23', 2, 0, 740.99),
    (4, 30, '2025-09-15', '2025-09-18', 2, 1, 860.80),
    (5, 31, '2025-05-18', '2025-05-20', 1, 0, 555.50),
    (1, 32, '2025-06-18', '2025-06-21', 3, 2, 1120.00),
    (2, 33, '2025-07-28', '2025-07-30', 2, 1, 675.75),
    (3, 34, '2025-08-25', '2025-08-28', 1, 1, 599.95);";

            string insertRoomAmenities = @"
            INSERT INTO room_amenities (room_id, amenity_id) VALUES
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

            ExecuteNonQuery(connection, insertRooms);
            ExecuteNonQuery(connection, insertUsers);
            ExecuteNonQuery(connection, insertReservations);
            ExecuteNonQuery(connection, insertRoomAmenities);
            InsertSampleImages(connection);
            // InsertAmenitiesImages(connection);
        }

        static void ExecuteNonQuery(SQLiteConnection connection, string query)
        {
            using (var cmd = new SQLiteCommand(query, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        static void InsertSampleImages(SQLiteConnection connection)
        {
            for (int i = 1; i <= 10; i++)
            {
                SaveImageToDatabase(i, "hotel_room.jpg", connection);
            }
        }

        static void InsertAmenitiesImages(SQLiteConnection connection)
        {
            for (int i = 1; i <= 10; i++)
            {
                string imagePath = $"icons/{i}.png";
                if (File.Exists(imagePath))
                {
                    byte[] imageBytes = File.ReadAllBytes(imagePath);
                    string query = "UPDATE amenities SET icon = @icon WHERE id = @id";
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", i);
                        cmd.Parameters.AddWithValue("@icon", imageBytes);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void SaveImageToDatabase(int roomId, string imagePath, SQLiteConnection connection)
        {
            if (File.Exists(imagePath))
            {
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                string query = "INSERT INTO images (room_id, image_data) VALUES (@roomId, @imageData)";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@roomId", roomId);
                    cmd.Parameters.AddWithValue("@imageData", imageBytes);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static ImageSource LoadImageFromDatabase(int roomId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT image_data FROM images WHERE room_id = @roomId";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@roomId", roomId);
                    var imageBytes = cmd.ExecuteScalar() as byte[];

                    return SourceFromByteArray(imageBytes);
                }
            }
        }

        public static Room[] GetAllRooms()
        {
            var roomList = new System.Collections.Generic.List<Room>();

            string query = "SELECT id, name, capacity, price_per_night, description FROM rooms";

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var room = new Room
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Capacity = reader.GetInt32(2),
                                PricePerNight = reader.GetFloat(3),
                                Description = reader.IsDBNull(4) ? "" : reader.GetString(4)
                            };
                            roomList.Add(room);
                        }
                    }
                }
            }
            return roomList.ToArray();
        }

        public static Room[] GetMatchingRooms(DateTime checkInDate, DateTime checkOutDate, int selectedAdults, int selectedChildren, string[] amenities)
        {
            List<Room> roomList = new List<Room>();
            //all amenities need to be available
            string query = @"
                    SELECT * 
                    FROM rooms 
                    WHERE id NOT IN (
                        SELECT room_id FROM reservations WHERE 
                        (check_in >= @checkInDate AND check_in <= @checkOutDate) AND
                        (check_out >= @checkInDate AND check_out <= @checkOutDate)
                    ) 
                    AND capacity >= @adults + @children";
            string amenitiesQuerry = @"
                    AND id IN (
                        SELECT room_id
                        FROM room_amenities ra JOIN amenities a ON ra.amenity_id = a.id
                        WHERE a.name IN (" + string.Join(", ", amenities.Select(s => $"\"{s}\"")) + @")
                        GROUP BY room_id
                        HAVING COUNT(DISTINCT a.name) = " + amenities.Length + @"
                    )";

            if (amenities.Length > 0)
            {
                query += amenitiesQuerry;
            }

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@checkInDate", checkInDate);
                    command.Parameters.AddWithValue("@checkOutDate", checkOutDate);
                    command.Parameters.AddWithValue("@adults", selectedAdults);
                    command.Parameters.AddWithValue("@children", selectedChildren);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var room = new Room
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Capacity = reader.GetInt32(2),
                                PricePerNight = reader.GetFloat(3),
                                Description = reader.IsDBNull(4) ? "" : reader.GetString(4)
                            };
                            roomList.Add(room);
                        }
                    }
                }
            }
            return roomList.ToArray();
        }

        public static Amenity[] GetAllAmenities()
        {
            List<Amenity> amenitiesList = new List<Amenity>();

            string query = "SELECT id, name, icon FROM amenities";

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var amenity = new Amenity
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Icon = reader.GetString(2)
                            };
                            amenitiesList.Add(amenity);
                        }
                    }
                }
            }
            return amenitiesList.ToArray();
        }

        public static Amenity[] GetAmenitiesForRoom(int roomId)
        {
            List<Amenity> amenitiesList = new List<Amenity>();

            string query = @"
            SELECT a.id, a.name, a.icon FROM amenities a
            JOIN room_amenities ra ON a.id = ra.amenity_id
            WHERE ra.room_id = @roomId";

            using (var connection = new SQLiteConnection(DatabaseManager.ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@roomId", roomId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var amenity = new Amenity
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Icon = reader.GetString(2)
                            };
                            amenitiesList.Add(amenity);
                        }
                    }
                }
            }
            return amenitiesList.ToArray();
        }

        public static ImageSource SourceFromByteArray(byte[] byteArray)
        {
            if (byteArray != null)
            {
                using (var ms = new MemoryStream(byteArray))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.EndInit();
                    return bitmapImage;
                }
            }
            return null;
        }

        public static ImageSource SourceFromFileName(string fileName)
        {
            if (File.Exists(fileName))
            {
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = fs;
                    bitmapImage.EndInit();
                    return bitmapImage;
                }
            }
            return null;
        }

        public static void UpdateRoom(int roomId, string name, int capacity, float pricePerNight, string description)
        {
            string query = "UPDATE rooms SET name = @name, capacity = @capacity, price_per_night = @price, description = @description WHERE id = @id";

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", roomId);
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@capacity", capacity);
                    command.Parameters.AddWithValue("@price", pricePerNight);
                    command.Parameters.AddWithValue("@description", description);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteAmenityFromRoom(int roomId, int amenityId)
        {
            string query = "DELETE FROM room_amenities WHERE room_id = @roomId AND amenity_id = @amenityId";
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@roomId", roomId);
                    command.Parameters.AddWithValue("@amenityId", amenityId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateRoomImage(int roomId, byte[] imageBytes)
        {
            string query = "UPDATE images SET image_data = @imageData WHERE room_id = @roomId";

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@roomId", roomId);
                    command.Parameters.AddWithValue("@imageData", imageBytes);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void AddAmenityToRoom(int roomId, int amenityId)
        {
            string query = "INSERT INTO room_amenities (room_id, amenity_id) VALUES (@roomId, @amenityId)";
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@roomId", roomId);
                    command.Parameters.AddWithValue("@amenityId", amenityId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static Room GetRoomWithId(int roomId)
        {
            string query = "SELECT id, name, capacity, price_per_night, description FROM rooms WHERE id = @roomId";
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@roomId", roomId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Room
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Capacity = reader.GetInt32(2),
                                PricePerNight = reader.GetFloat(3),
                                Description = reader.IsDBNull(4) ? "" : reader.GetString(4)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static ReservationCard[] GetFilteredReservations(string searchText)
        {
            List<ReservationCard> reservationCards = new List<ReservationCard>();

            string query = @"
            SELECT *
            FROM reservations r
            JOIN users u ON r.user_id = u.id
            JOIN rooms ro ON r.room_id = ro.id
            ";

            if (!string.IsNullOrEmpty(searchText))
            {
                query += " WHERE ";

                searchText.Split(' ').ToList().ForEach(s =>
                {
                    s.Trim();
                    query += $"(u.name LIKE '%{s}%' OR u.surname LIKE '%{s}%' OR ro.name LIKE '%{s}%' OR u.email LIKE '%{s}%' OR u.phone LIKE '%{s}%') OR";
                });
                query = query.Substring(0, query.Length - 3); // Remove the last OR
            }
            query += " ORDER BY r.check_in DESC";

            // MessageBox.Show($"Searching for: {query}");

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //show full response


                            var reservation = new Reservation
                            {
                                Id = reader.GetInt32(0),
                                RoomId = reader.GetInt32(1),
                                UserId = reader.GetInt32(2),
                                CheckIn = reader.GetDateTime(3),
                                CheckOut = reader.GetDateTime(4),
                                Adults = reader.GetInt32(5),
                                Children = reader.GetInt32(6),
                                TotalPrice = reader.GetFloat(7)
                            };
                            var user = new User
                            {
                                Id = reader.GetInt32(8),
                                Name = reader.GetString(9),
                                Surname = reader.GetString(10),
                                Email = reader.GetString(11),
                                Phone = reader.IsDBNull(12) ? "" : reader.GetString(12)
                            };
                            var room = new Room
                            {
                                Id = reader.GetInt32(13),
                                Name = reader.GetString(14),
                                Capacity = reader.GetInt32(15),
                                PricePerNight = reader.GetFloat(16),
                                Description = reader.IsDBNull(17) ? "" : reader.GetString(17)
                            };
                            ReservationCard reservationCard = new ReservationCard
                            {
                                Reservation = reservation,
                                Room = room,
                                User = user
                            };

                            reservationCards.Add(reservationCard);

                            // Add reservation to the UI or process it as needed
                        }
                    }
                }
            }

            return reservationCards.ToArray();
        }

        public static Guest[] GetGuestsByReservationId(int reservationId)
        {
            List<Guest> guests = new List<Guest>();

            string query = @"
            SELECT g.id, g.name, g.surname
            FROM guests g
            JOIN guest_reservations gr ON g.id = gr.guest_id
            WHERE gr.reservation_id = @reservationId";

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@reservationId", reservationId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var guest = new Guest
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Surname = reader.GetString(2)
                            };
                            guests.Add(guest);
                        }
                    }
                }
            }
            return guests.ToArray();
        }

        public static int InsertRoom(Room room)
        {
            string query = "INSERT INTO rooms (name, capacity, price_per_night, description) VALUES (@name, @capacity, @price_per_night, @description)";

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", room.Name);
                    command.Parameters.AddWithValue("@capacity", room.Capacity);
                    command.Parameters.AddWithValue("@price_per_night", room.PricePerNight);
                    command.Parameters.AddWithValue("@description", room.Description);
                    command.ExecuteNonQuery();
                }

                return GetLastInsertedId(connection);
            }
        }

        public static void InsertImage(int roomId, byte[] imageBytes)
        {
            string query = "INSERT INTO images (room_id, image_data) VALUES (@roomId, @imageData)";

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@roomId", roomId);
                    command.Parameters.AddWithValue("@imageData", imageBytes);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static int GetLastInsertedId(SQLiteConnection connection)
        {
            string query = "SELECT last_insert_rowid()";

            using (var command = new SQLiteCommand(query, connection))
            {
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        //         Reservation reservations = DatabaseManager.GetReservationsForRoom(RoomId);

        // DatabaseManager.DeleteRoom(RoomId);
        // DatabaseManager.DeleteImageWithRoomId(RoomId);
        // DatabaseManager.DeleteAllAmenitiesFromRoom(RoomId);

        public static void DeleteRoom(int roomId)
        {
            string query = "DELETE FROM rooms WHERE id = @roomId";

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@roomId", roomId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteImageWithRoomId(int roomId)
        {
            string query = "DELETE FROM images WHERE room_id = @roomId";

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@roomId", roomId);
                    command.ExecuteNonQuery();
                }
            }
        }
        public static void DeleteAllAmenitiesFromRoom(int roomId)
        {
            string query = "DELETE FROM room_amenities WHERE room_id = @roomId";

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@roomId", roomId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static Reservation[] GetReservationsForRoom(int roomId)
        {
            string query = "SELECT * FROM reservations WHERE room_id = @roomId";

            List<Reservation> reservationList = new List<Reservation>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@roomId", roomId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var reservation = new Reservation
                            {
                                Id = reader.GetInt32(0),
                                RoomId = reader.GetInt32(1),
                                UserId = reader.GetInt32(2),
                                CheckIn = reader.GetDateTime(3),
                                CheckOut = reader.GetDateTime(4),
                                Adults = reader.GetInt32(5),
                                Children = reader.GetInt32(6),
                                TotalPrice = reader.GetFloat(7)
                            };
                            reservationList.Add(reservation);
                        }
                    }
                }
            }

            return reservationList.ToArray();
        }

        public static void InsertAmenity(Amenity amenity)
        {
            string query = "INSERT INTO amenities (name, icon) VALUES (@name, @icon)";

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", amenity.Name);
                    command.Parameters.AddWithValue("@icon", amenity.Icon);
                    command.ExecuteNonQuery();
                }
            }
        }
        public static void DeleteAmenity(int amenityId)
        {
            string query = "DELETE FROM amenities WHERE id = @amenityId";

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@amenityId", amenityId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateAmenity(int amenityId, string name, string icon)
        {
            string query = "UPDATE amenities SET name = @name, icon = @icon WHERE id = @amenityId";

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@amenityId", amenityId);
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@icon", icon);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static Room[] GetRoomsByAmenityId(int amenityId)
        {
            List<Room> roomList = new List<Room>();

            string query = @"
            SELECT r.id, r.name, r.capacity, r.price_per_night, r.description
            FROM rooms r
            JOIN room_amenities ra ON r.id = ra.room_id
            WHERE ra.amenity_id = @amenityId";

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@amenityId", amenityId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var room = new Room
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Capacity = reader.GetInt32(2),
                                PricePerNight = reader.GetFloat(3),
                                Description = reader.IsDBNull(4) ? "" : reader.GetString(4)
                            };
                            roomList.Add(room);
                        }
                    }
                }
            }
            return roomList.ToArray();
        }
    }
}