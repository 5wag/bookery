using CommandLine;

namespace Bookery;

internal class CommandLineArgs
{
    [Option(longName: "hotels", Required = true, HelpText = "Path to location of json file with data for hotels.")]
    public required string HotelsFilePath { get; set; }

    [Option(longName: "bookings", Required = true, HelpText = "Path to location of json file with data for bookings.")]
    public required string BookingsFilePath { get; set; }
}
