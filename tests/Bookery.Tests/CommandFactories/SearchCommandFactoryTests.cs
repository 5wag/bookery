using Bookery.CommandFactories;

namespace Bookery.Tests.CommandFactories;

public class SearchCommandFactoryTests
{
    [Theory]
    [InlineData("A1b2", "365", "XyZ9", "   ,   ")]
    [InlineData("3C3d4", "1", "AbC8", "   ,")]
    [InlineData("E5f6", "1000", "MnO7", ",   ")]
    [InlineData("G7h8", "2", "PqR6", ",")]
    [InlineData("G", "2", "PqR6", ",")]
    [InlineData("1", "2", "PqR6", ",")]
    public void ShouldCreateSearchCommand(string hotelId, string days, string roomType, string sep)
    {
        var factory = new SearchCommandFactory();
        var commandString = $"Search({hotelId}{sep}{days}{sep}{roomType})";

        var canHandle = factory.TryCreate(commandString, out var command);

        // Assert
        Assert.True(canHandle);
        Assert.IsType<SearchCommand>(command);
        var searchCommand = (SearchCommand)command;
        Assert.Equal(hotelId, searchCommand.HotelId);
        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), searchCommand.From);
        Assert.Equal(int.Parse(days), searchCommand.Days);
        Assert.Equal(roomType, searchCommand.RoomType);
    }

    [Theory]
    [InlineData("Search(H1, 365, SGL)", true)]
    [InlineData("Search(H2, 30, DBL)", true)]
    [InlineData("Search(H2, 0, DBL)", false)]
    [InlineData("InvalidCommand(H1, 365, SGL)", false)]
    [InlineData("Search(, 365, SGL)", false)]
    [InlineData("Search(H1, , SGL)", false)]
    [InlineData("Search(H1, 365, )", false)]
    [InlineData("Search(H1, 365)", false)]
    [InlineData("Search(H1, 365, SGL, Extra)", false)]
    public void ShouldBeAbleToHandleSearchCommand(string commandString, bool expected)
    {
        var factory = new SearchCommandFactory();

        var canHandle = factory.TryCreate(commandString, out var _);

        Assert.Equal(expected, canHandle);
    }
}