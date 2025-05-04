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
    name TEXT NOT NULL UNIQUE,
    icon BLOB
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

INSERT INTO amenities (name) VALUES
('WiFi'),
('Private Bathroom'),
('Air Conditioning'),
('Heating'),
('TV'),
('Towels'),
('Toiletries'),
('Hair Dryer'),
('Refrigerator'),
('Coffee Maker');
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
                ('Bob', 'Johnson', 'bob.johnson@example.com', '555-555-5555'); ";

            string insertReservations = @"
                INSERT INTO reservations (room_id, user_id, check_in, check_out, adults, children) VALUES
                (1, 1, '2025-05-10', '2025-05-12', 1, 0), (2, 2, '2025-05-15', '2025-05-18', 2, 1);";

            string insertRoomAmenities = @"
            INSERT INTO room_amenities (room_id, amenity_id) VALUES
                (1, 1), (1, 2), (1, 3),
                (2, 1), (2, 3), (2, 5), (2, 6),
                (3, 1), (3, 2), (3, 4), (3, 7), (3, 10),
                (4, 1), (4, 3), (4, 5), (4, 9), (4, 13),
                (5, 1), (5, 2), (5, 6), (5, 7),
                (6, 1), (6, 4), (6, 5), (6, 11),
                (7, 1), (7, 3), (7, 12),
                (8, 1), (8, 2), (8, 6),
                (9, 1), (9, 4), (9, 5), (9, 10),
                (10, 1), (10, 3), (10, 8), (10, 13), (10, 14);
                ";

            ExecuteNonQuery(connection, insertRooms);
            ExecuteNonQuery(connection, insertUsers);
            ExecuteNonQuery(connection, insertReservations);
            ExecuteNonQuery(connection, insertRoomAmenities);
            InsertSampleImages(connection);
            InsertAmenitiesImages(connection);
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
            var roomList = new System.Collections.Generic.List<Room>();
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
            var amenitiesList = new System.Collections.Generic.List<Amenity>();

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
                                Icon = reader.IsDBNull(2) ? null : (byte[])reader[2]
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
            var amenitiesList = new System.Collections.Generic.List<Amenity>();

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
                                Icon = reader.IsDBNull(2) ? null : (byte[])reader[2]
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

    }
}