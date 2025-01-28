using Bookery.Data;

namespace Bookery.Commands;

public class AvailabilityCommand : IUserCommand
{
    public required string HotelId { get; init; }
    public required DateOnly Arrival { get; init; }
    public required DateOnly Departure { get; init; }
    public required string RoomType { get; init; }
    
    public string Execute(DataStore data)
    {
        var maxRooms = data.Hotels.Where(x => x.Id == HotelId).SelectMany(x => x.Rooms).Count(x => x.RoomType == RoomType);
        if (maxRooms == 0) return "0";

        var bookings = data.Bookings
            .Where(x => x.HotelId == HotelId && x.RoomType == RoomType)
            .ToList();

        if (bookings.Count == 0) return maxRooms.ToString();

        var rooms = maxRooms;
        var today = Arrival;
        do
        {
            var bookingsToday = bookings.Count(x => x.Arrival <= today && x.Departure > today);
            rooms = Math.Min(rooms, maxRooms - bookingsToday);
            today = today.AddDays(1);
        }
        while (today != Departure);

        return rooms.ToString();
    }
}
