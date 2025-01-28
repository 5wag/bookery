using System.Text.Json.Serialization;

namespace Bookery;

public record RoomType([property: JsonPropertyName("code")] string Code);

public record Room([property: JsonPropertyName("roomType")] string RoomType);

public record Hotel([property: JsonPropertyName("id")] string Id, 
                    [property: JsonPropertyName("rooms")] Room[] Rooms);

public record Booking([property: JsonPropertyName("hotelId")] string HotelId, 
                      [property: JsonPropertyName("arrival")] DateOnly Arrival, 
                      [property: JsonPropertyName("departure")] DateOnly Departure, 
                      [property: JsonPropertyName("roomType")] string RoomType);