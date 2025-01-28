using Bookery.Data;

namespace Bookery.Commands;

public class SearchCommand : IUserCommand
{
    public required string HotelId { get; init; }
    public required DateOnly From { get; set; }
    public required int Days { get; init; }
    public required string RoomType { get; init; }

    public string Execute(DataStore data)
    {
        var rooms = data.Hotels.Where(x => x.Id == HotelId).SelectMany(x => x.Rooms).Count(x => x.RoomType == RoomType);
        if (rooms == 0) return string.Empty;

        var bookings = data.Bookings.Where(x => x.HotelId == HotelId && x.RoomType == RoomType).ToList();

        var firstDay = From;
        var lastDay = From.AddDays(Days);

        List<AvailabilityInfo> availabilities = new();
        AvailabilityInfo availability = new()
        {
            Arrival = firstDay,
            Departure = firstDay,
            Rooms = 0
        };
        var currentDay = firstDay;
        for (int i = 0; i < Days; i++)
        {
            var bs = bookings.Count(x => Booked(x, currentDay));
            if (availability.Rooms != rooms - bs)
            {
                if (availability.Rooms == 0)
                {
                    if (rooms - bs > 0)
                    {
                        availability = new AvailabilityInfo
                        {
                            Arrival = currentDay,
                            Departure = currentDay,
                            Rooms = rooms - bs
                        };
                    }
                }
                else if (availability.Arrival != availability.Departure)
                {
                    availabilities.Add(availability);

                    availability = new AvailabilityInfo
                    {
                        Arrival = currentDay,
                        Departure = currentDay,
                        Rooms = rooms - bs > 0 ? rooms - bs : 0
                    };
                }
            }

            if (i == Days - 1 && availability.Rooms != 0)
            {
                availabilities.Add(availability);
            }

            currentDay = currentDay.AddDays(1);
            availability.Departure = currentDay;
        }

        return string.Join(", ", availabilities.Select(x => $"({x.Arrival:yyyyMMdd}-{x.Departure:yyyyMMdd}, {x.Rooms})"));
    }

    private static bool Booked(Booking booking, DateOnly day)
    {
        if (booking.Arrival <= day && booking.Departure > day)
        {
            return true;
        }

        return false;
    }

    class AvailabilityInfo
    {
        public DateOnly Arrival { get; set; }
        public DateOnly Departure { get; set; }
        public int Rooms { get; set; }
    }
}
