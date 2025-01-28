using Bookery.CommandFactories;

namespace Bookery.Tests.CommandFactories;

public class AvailabilityCommandFactoryTests
{
    [Theory]
    [InlineData("hotelid", "20250101", "20250102", "roomtype", ", ")]
    [InlineData("1111", "20250101", "20250102", "222", ",")]
    [InlineData("LetterS123", "20250101", "20250102", "123RoomType123", " , ")]
    [InlineData("hotelid", "21000101", "21010101", "roomtype", " ,")]
    public void ShouldCreateExpectedCommand(string hotelId, string arrival, string departure, string roomType, string sep)
    {
        var factory = new AvailabilityCommandFactory();
        var commandString = $"Availability({hotelId}{sep}{arrival}-{departure}{sep}{roomType})";

        var canHandle = factory.TryCreate(commandString, out var command);

        Assert.True(canHandle);
        Assert.IsType<AvailabilityCommand>(command);
        var availabilityCommand = (AvailabilityCommand)command;
        Assert.Equal(hotelId, availabilityCommand.HotelId);
        Assert.Equal(arrival, availabilityCommand.Arrival.ToString("yyyyMMdd"));
        Assert.Equal(departure, availabilityCommand.Departure.ToString("yyyyMMdd"));
        Assert.Equal(roomType, availabilityCommand.RoomType);
    }

    [Theory]
    [InlineData("hotelid", "20250101", "roomtype", ",   ")]
    [InlineData("1111", "20250101", "222", "  ,  ")]
    [InlineData("LetterS123", "20250101", "123RoomType123", "   ,")]
    [InlineData("hotelid", "21000101", "roomtype", ",")]
    public void ShouldCreateExpectedCommandWhenNoDeparture(string hotelId, string arrival, string roomType, string sep)
    {
        var factory = new AvailabilityCommandFactory();
        var commandString = $"Availability({hotelId}{sep}{arrival}{sep}{roomType})";

        var canHandle = factory.TryCreate(commandString, out var command);

        Assert.True(canHandle);
        Assert.IsType<AvailabilityCommand>(command);
        var availabilityCommand = (AvailabilityCommand)command;
        Assert.Equal(hotelId, availabilityCommand.HotelId);
        Assert.Equal(arrival, availabilityCommand.Arrival.ToString("yyyyMMdd"));
        Assert.Equal(roomType, availabilityCommand.RoomType);
        var departure = DateOnly.ParseExact(arrival, "yyyyMMdd").AddDays(1);
        Assert.Equal(departure, availabilityCommand.Departure);
    }

    [Theory]
    [InlineData("Availability(h, 20250101-20250102 , r)", true)]
    [InlineData("Availability(1,20250101-20250102 , 1)", true)]
    [InlineData("Availability(LetterS123 ,20250101-20250102  ,   123RoomType123)", true)]
    [InlineData("Availability(hotelid      , 21000101-21010101   , roomtype)", true)]
    [InlineData("Availability(1,20250101-20241231, 1)", false)]
    [InlineData("Availability(1,20250140-20250201, 1)", false)]
    [InlineData("Availability(1,20250101-20250230, 1)", false)]
    [InlineData("Availability(1, 1)", false)]
    [InlineData("Availability(, 21000101, roomtype)", false)]
    [InlineData("Availability(,,)", false)]
    [InlineData("Availability(hotelid,, roomtype)", false)]
    [InlineData("Availability(!, 21000101, roomtype)", false)]
    [InlineData("Availability(h, 21000101, !)", false)]
    [InlineData("availability(h, 21000101, !)", false)]
    [InlineData("Availabilit(h, 21000101, !)", false)]
    [InlineData("random(h, 21000101, 1)", false)]
    public void ShouldBeAbleToHandleCommand(string commandString, bool expected)
    {
        var factory = new AvailabilityCommandFactory();

        var canHandle = factory.TryCreate(commandString, out var _);

        Assert.Equal(expected, canHandle);
    }
}
