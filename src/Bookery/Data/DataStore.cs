namespace Bookery.Data;

public class DataStore
{
    public required ImmutableArray<Hotel> Hotels { get; init; }
    public required ImmutableArray<Booking> Bookings { get; init; }
}
