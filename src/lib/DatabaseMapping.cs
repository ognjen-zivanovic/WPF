
using Dapper;
using System;
using System.Reflection;

namespace HotelRezervacije
{
    public static class DatabaseMapping
    {
        public static void ConfigureMappings()
        {
            // Room
            SqlMapper.SetTypeMap(typeof(Room), new CustomPropertyTypeMap(
                typeof(Room),
                (type, column) => column switch
                {
                    "id" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "capacity" => type.GetProperty("Capacity"),
                    "price_per_night" => type.GetProperty("PricePerNight"),
                    "description" => type.GetProperty("Description"),
                    _ => null
                }));

            // User
            SqlMapper.SetTypeMap(typeof(User), new CustomPropertyTypeMap(
                typeof(User),
                (type, column) => column switch
                {
                    "id" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "surname" => type.GetProperty("Surname"),
                    "email" => type.GetProperty("Email"),
                    "phone" => type.GetProperty("Phone"),
                    _ => null
                }));

            // Reservation
            SqlMapper.SetTypeMap(typeof(Reservation), new CustomPropertyTypeMap(
                typeof(Reservation),
                (type, column) => column switch
                {
                    "id" => type.GetProperty("Id"),
                    "room_id" => type.GetProperty("RoomId"),
                    "user_id" => type.GetProperty("UserId"),
                    "check_in" => type.GetProperty("CheckIn"),
                    "check_out" => type.GetProperty("CheckOut"),
                    "number_of_guests" => type.GetProperty("NumberOfGuests"),
                    "total_price" => type.GetProperty("TotalPrice"),
                    _ => null
                }));

            // DatabaseImage
            SqlMapper.SetTypeMap(typeof(DatabaseImage), new CustomPropertyTypeMap(
                typeof(DatabaseImage),
                (type, column) => column switch
                {
                    "id" => type.GetProperty("Id"),
                    "room_id" => type.GetProperty("RoomId"),
                    "image_data" => type.GetProperty("ImageData"),
                    _ => null
                }));

            // Amenity
            SqlMapper.SetTypeMap(typeof(Amenity), new CustomPropertyTypeMap(
                typeof(Amenity),
                (type, column) => column switch
                {
                    "id" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "icon" => type.GetProperty("Icon"),
                    _ => null
                }));

            // Guest
            SqlMapper.SetTypeMap(typeof(Guest), new CustomPropertyTypeMap(
                typeof(Guest),
                (type, column) => column switch
                {
                    "id" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "surname" => type.GetProperty("Surname"),
                    _ => null
                }));

            // GuestReservations
            SqlMapper.SetTypeMap(typeof(GuestReservations), new CustomPropertyTypeMap(
                typeof(GuestReservations),
                (type, column) => column switch
                {
                    "id" => type.GetProperty("Id"),
                    "reservation_id" => type.GetProperty("ReservationId"),
                    "guest_id" => type.GetProperty("GuestId"),
                    _ => null
                }));
        }
    }
}
