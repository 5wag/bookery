using Bookery.Data;

namespace Bookery.Tests.Commands;

public class SearchCommandTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    public void ShouldReturnExpectedResult_NoBookingsNoRooms(int days)
    {
        var hotel = new Hotel("H1", []);
        ImmutableArray<Booking> bookings = [];
        var data = new DataStore { Hotels = [hotel], Bookings = bookings };
        var command = new SearchCommand
        {
            HotelId = "H1",
            From = new DateOnly(2025, 01, 01),
            Days = days,
            RoomType = "A"
        };

        var result = command.Execute(data);

        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("H1", "A", "H2", "A")]
    [InlineData("H1", "A", "H1", "B")]
    [InlineData("H1", "B", "H1", "A")]
    [InlineData("H2", "A", "H1", "A")]
    public void ShouldReturnExpectedResult_MismatchOfHotelOrRoomType(string hotelId, string roomType, string commandHotelId, string commandRoomType)
    {
        var hotel = new Hotel(hotelId, [new Room(roomType)]);
        ImmutableArray<Booking> bookings = [];
        var data = new DataStore { Hotels = [hotel], Bookings = bookings };
        var command = new SearchCommand
        {
            HotelId = commandHotelId,
            From = new DateOnly(2025, 01, 01),
            Days = 5,
            RoomType = commandRoomType
        };

        var result = command.Execute(data);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ShouldReturnExpectedResult_AllBooked()
    {
        var hotel = new Hotel("H1", [new Room("A")]);
        ImmutableArray<Booking> bookings = [new("H1", new DateOnly(2024, 01, 01), new DateOnly(2026, 01, 01), "A")];
        var data = new DataStore { Hotels = [hotel], Bookings = bookings };
        var command = new SearchCommand
        {
            HotelId = "H1",
            From = new DateOnly(2025, 01, 01),
            Days = 5,
            RoomType = "A"
        };

        var result = command.Execute(data);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ShouldReturnExpectedResult_MoreBookingsThanRooms()
    {
        var hotel = new Hotel("H1", [new Room("A"), new Room("A")]);
        ImmutableArray<Booking> bookings = [
            new("H1", new DateOnly(2024, 01, 01), new DateOnly(2026, 01, 01), "A"),
            new("H1", new DateOnly(2024, 01, 01), new DateOnly(2026, 01, 01), "A"),
            new("H1", new DateOnly(2024, 01, 01), new DateOnly(2026, 01, 01), "A"),
            ];
        var data = new DataStore { Hotels = [hotel], Bookings = bookings };
        var command = new SearchCommand
        {
            HotelId = "H1",
            From = new DateOnly(2025, 01, 01),
            Days = 5,
            RoomType = "A"
        };

        var result = command.Execute(data);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ShouldReturnExpectedResult_BookingsBeforeAndAfter()
    {
        var hotel = new Hotel("H1", [new Room("A"), new Room("A"), new Room("A")]);
        ImmutableArray<Booking> bookings = [
            new("H1", new DateOnly(2024, 01, 01), new DateOnly(2025, 01, 01), "A"),
            new("H1", new DateOnly(2025, 01, 06), new DateOnly(2025, 02, 06), "A"),
        ];
        var data = new DataStore { Hotels = [hotel], Bookings = bookings };
        var command = new SearchCommand
        {
            HotelId = "H1",
            From = new DateOnly(2025, 01, 01),
            Days = 5,
            RoomType = "A"
        };

        var result = command.Execute(data);

        Assert.Equal("(20250101-20250106, 3)", result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public void ShouldReturnExpectedResult_NoBookingsMultipleRooms(int rooms)
    {
        var roomsA = Enumerable.Range(0, rooms).Select(x => new Room("A"));
        var hotel = new Hotel("H1", [.. roomsA, new Room("B")]);
        ImmutableArray<Booking> bookings = [];
        var data = new DataStore { Hotels = [hotel], Bookings = bookings };
        var command = new SearchCommand
        {
            HotelId = "H1",
            From = new DateOnly(2025, 01, 01),
            Days = 3,
            RoomType = "A"
        };

        var result = command.Execute(data);

        Assert.Equal($"(20250101-20250104, {rooms})", result);
    }

    [Fact]
    public void ShouldReturnExpectedResult_BookingsIncrease()
    {
        var hotel = new Hotel("H1", [new Room("A")]);
        ImmutableArray<Booking> bookings = [
            new("H1", new DateOnly(2025, 01, 01), new DateOnly(2025, 01, 03), "A"),
            new("H1", new DateOnly(2025, 01, 02), new DateOnly(2025, 01, 03), "A")
        ];
        var data = new DataStore { Hotels = [hotel], Bookings = bookings };
        var command = new SearchCommand
        {
            HotelId = "H1",
            From = new DateOnly(2025, 01, 01),
            Days = 3,
            RoomType = "A"
        };

        var result = command.Execute(data);

        Assert.Equal("(20250103-20250104, 1)", result);
    }

    [Fact]
    public void ShouldReturnExpectedResult_BookingsIncrease2()
    {
        var hotel = new Hotel("H1", [new Room("A"), new Room("A"), new Room("A")]);
        ImmutableArray<Booking> bookings = [
            new("H1", new DateOnly(2024, 12, 31), new DateOnly(2025, 01, 02), "A"),
            new("H1", new DateOnly(2025, 01, 01), new DateOnly(2025, 01, 03), "A")
        ];
        var data = new DataStore { Hotels = [hotel], Bookings = bookings };
        var command = new SearchCommand
        {
            HotelId = "H1",
            From = new DateOnly(2025, 01, 01),
            Days = 3,
            RoomType = "A"
        };

        var result = command.Execute(data);

        Assert.Equal("(20250101-20250102, 1), (20250102-20250103, 2), (20250103-20250104, 3)", result);
    }

    [Fact]
    public void ShouldReturnExpectedResult_BookingsDecrease()
    {
        var hotel = new Hotel("H1", [new Room("A"), new Room("A"), new Room("A")]);
        ImmutableArray<Booking> bookings = [
            new("H1", new DateOnly(2025, 01, 01), new DateOnly(2025, 01, 04), "A"),
            new("H1", new DateOnly(2025, 01, 01), new DateOnly(2025, 01, 03), "A"),
            new("H1", new DateOnly(2025, 01, 01), new DateOnly(2025, 01, 02), "A"),
        ];
        var data = new DataStore { Hotels = [hotel], Bookings = bookings };
        var command = new SearchCommand
        {
            HotelId = "H1",
            From = new DateOnly(2025, 01, 01),
            Days = 3,
            RoomType = "A"
        };

        var result = command.Execute(data);

        Assert.Equal("(20250102-20250103, 1), (20250103-20250104, 2)", result);
    }

    [Fact]
    public void ShouldReturnExpectedResult_BookingsWithIntervals()
    {
        var hotel = new Hotel("H1", [new Room("A"), new Room("A"), new Room("A")]);
        ImmutableArray<Booking> bookings = [
            new("H1", new DateOnly(2025, 01, 01), new DateOnly(2025, 01, 03), "A"),
            new("H1", new DateOnly(2025, 01, 01), new DateOnly(2025, 01, 11), "A"),
            new("H1", new DateOnly(2025, 01, 08), new DateOnly(2025, 01, 11), "A"),
        ];
        var data = new DataStore { Hotels = [hotel], Bookings = bookings };
        var command = new SearchCommand
        {
            HotelId = "H1",
            From = new DateOnly(2025, 01, 01),
            Days = 10,
            RoomType = "A"
        };

        var result = command.Execute(data);

        Assert.Equal("(20250101-20250103, 1), (20250103-20250108, 2), (20250108-20250111, 1)", result);
    }
}