using Bookery.Data;

namespace Bookery.Tests.Commands;

public class AvailabilityCommandTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    public void ShouldReturnExpectedResult_NoRoomsNoBookings(int roomsCount)
    {
        var rooms = Enumerable.Range(0, roomsCount).Select(_ => new Room("A")).ToArray();
        var data = new DataStore { Bookings = [], Hotels = [new("H1", rooms)] };
        var command = new AvailabilityCommand
        {
            Arrival = new DateOnly(2025, 01, 01),
            Departure = new DateOnly(2025, 01, 03),
            HotelId = "H1",
            RoomType = "A"
        };

        var result = command.Execute(data);

        Assert.Equal(roomsCount.ToString(), result);
    }

    [Theory]
    [InlineData("H1", "A", "H2", "A")]
    [InlineData("H1", "A", "H1", "B")]
    [InlineData("H1", "B", "H1", "A")]
    [InlineData("H2", "A", "H1", "A")]
    public void ShouldReturnExpectedResult_WhenParamsMismatch(string hotelId, string roomType, string commandHotelId, string commandRoomType)
    {
        var data = new DataStore { Bookings = [], Hotels = [new(hotelId, [new(roomType)])] };
        var command = new AvailabilityCommand
        {
            Arrival = new DateOnly(2025, 01, 01),
            Departure = new DateOnly(2025, 01, 03),
            HotelId = commandHotelId,
            RoomType = commandRoomType
        };

        var result = command.Execute(data);

        Assert.Equal("0", result);
    }

    [Fact]
    public void ShouldReturnExpectedResult_BookingInTheMiddle()
    {
        var hotel = new Hotel("H1", [new("A"), new("A"), new("A"), new("A")]);
        ImmutableArray<Booking> bookings = [
            new("H1", new DateOnly(2025, 01, 02), new DateOnly(2025, 01, 03), "A"),
        ];
        var data = new DataStore { Bookings = bookings, Hotels = [hotel] };
        var command = new AvailabilityCommand
        {
            Arrival = new DateOnly(2025, 01, 01),
            Departure = new DateOnly(2025, 01, 04),
            HotelId = "H1",
            RoomType = "A"
        };

        var result = command.Execute(data);

        Assert.Equal("3", result);
    }

    [Fact]
    public void ShouldReturnExpectedResult_BookingInTheBeginningAndExhaustiveBookingInTheEnd()
    {
        var hotel = new Hotel("H1", [new("A"), new("A")]);
        ImmutableArray<Booking> bookings = [
            new("H1", new DateOnly(2025, 01, 01), new DateOnly(2025, 01, 02), "A"),
            new("H1", new DateOnly(2025, 01, 02), new DateOnly(2025, 01, 04), "A"),
            new("H1", new DateOnly(2025, 01, 02), new DateOnly(2025, 01, 04), "A"),
        ];
        var data = new DataStore { Bookings = bookings, Hotels = [hotel] };
        var command = new AvailabilityCommand
        {
            Arrival = new DateOnly(2025, 01, 01),
            Departure = new DateOnly(2025, 01, 04),
            HotelId = "H1",
            RoomType = "A"
        };

        var result = command.Execute(data);

        Assert.Equal("0", result);
    }

    [Fact]
    public void ShouldReturnExpectedResult_BookingsOnEdge()
    {
        var hotel = new Hotel("H1", [new("A"), new("A"), new("A"), new("A")]);
        ImmutableArray<Booking> bookings = [
            new("H1", new DateOnly(2025, 12, 31), new DateOnly(2025, 01, 01), "A"),
            new("H1", new DateOnly(2025, 01, 01), new DateOnly(2025, 01, 02), "A"),
            new("H1", new DateOnly(2025, 01, 02), new DateOnly(2025, 01, 03), "A"),
            new("H1", new DateOnly(2025, 01, 03), new DateOnly(2025, 01, 04), "A"),
        ];
        var data = new DataStore { Bookings = bookings, Hotels = [hotel] };
        var command = new AvailabilityCommand
        {
            Arrival = new DateOnly(2025, 01, 01),
            Departure = new DateOnly(2025, 01, 03),
            HotelId = "H1",
            RoomType = "A"
        };

        var result = command.Execute(data);

        Assert.Equal("3", result);
    }

    [Fact]
    public void ShouldReturnExpectedResult_BookingsOverlap()
    {
        var hotel = new Hotel("H1", [new("A"), new("A"), new("A"), new("A")]);
        ImmutableArray<Booking> bookings = [
            new("H1", new DateOnly(2025, 01, 01), new DateOnly(2025, 01, 03), "A"),
            new("H1", new DateOnly(2025, 01, 01), new DateOnly(2025, 01, 02), "A"),
            new("H1", new DateOnly(2025, 01, 02), new DateOnly(2025, 01, 03), "A"),
        ];
        var data = new DataStore { Bookings = bookings, Hotels = [hotel] };
        var command = new AvailabilityCommand
        {
            Arrival = new DateOnly(2025, 01, 01),
            Departure = new DateOnly(2025, 01, 03),
            HotelId = "H1",
            RoomType = "A"
        };

        var result = command.Execute(data);

        Assert.Equal("2", result);
    }
}