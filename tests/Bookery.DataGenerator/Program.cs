using System.Text;
using System.Text.Json;
using Bookery;
using Bookery.Data;
using Bookery.DataGenerator;


var (hotelsCount, bookingsPerHotelCount) = (
    int.Parse(Environment.GetEnvironmentVariable("HOTELS_COUNT_MAX")!),
    int.Parse(Environment.GetEnvironmentVariable("BOOKINGS_COUNT_MAX")!));
var saveDir = Environment.GetEnvironmentVariable("SAVE_DIR") ?? throw new Exception("set SAVE_DIR env variable with location where to save files");

List<string> roomTypes = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J"];

var hotels = Enumerable.Range(1, hotelsCount + 1)
    .Select(x => new Hotel(
        Id: $"H{x}",
        Rooms: Enumerable.Range(1, Random.Shared.Next(20, 200) + 1)
            .Select(y => new Room(
                RoomType: roomTypes.Choose()
            ))
            .ToArray()
    ))
    .ToList();

var now = DateTime.Now;

var bookings = hotels
    .SelectMany(h => h.Rooms
        .SelectMany(r => Enumerable.Range(0, Random.Shared.Next(1, bookingsPerHotelCount + 1))
                .Select(_ =>
                {
                    var arrival = now.AddDays(Random.Shared.Next(1, 365));
                    var departure = arrival.AddDays(Random.Shared.Next(1, 30));
                    return new Booking(
                        HotelId: h.Id,
                        RoomType: r.RoomType,
                        Arrival: DateOnly.FromDateTime(arrival),
                        Departure: DateOnly.FromDateTime(departure));
                })
        ))
    .ToList();

var dir = Directory.CreateDirectory(saveDir);
var options = new JsonSerializerOptions{Converters = { new DateOnlyJsonConverter() } };
File.WriteAllText(Path.Combine(dir.FullName, "Hotels.json"), JsonSerializer.Serialize(hotels, options), Encoding.UTF8);
File.WriteAllText(Path.Combine(dir.FullName, "Bookings.json"), JsonSerializer.Serialize(bookings, options), Encoding.UTF8);