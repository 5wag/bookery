using Bookery.Commands;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Bookery.CommandFactories;

public class AvailabilityCommandFactory : IUserCommandFactory
{
    /// <example>
    /// Availability(H1, 20240901, SGL)
    /// Availability(H1, 20240901-20240903, DBL)
    /// </example>
    private readonly Regex _commandRegex = new Regex(@"^Availability\(([a-zA-Z\d]+)\s*,\s*(\d{8})(?:-(\d{8}))?\s*,\s*([a-zA-Z\d]+)\)$");

    public bool TryCreate(string commandString, [NotNullWhen(true)] out IUserCommand? command)
    {
        command = null;
        var match = _commandRegex.Match(commandString);
        if (!match.Success)
        {
            return false;
        }
        
        var hotelId = match.Groups[1].Value;
        
        DateOnly arrival;
        if (!TryParseDateOnly(match.Groups[2].Value, out arrival))
        {
            return false;
        }

        DateOnly departure;
        if (match.Groups[3].Success)
        {
            if (!TryParseDateOnly(match.Groups[3].Value, out departure) || departure <= arrival)
            {
                return false;
            }
        }
        else
        {
            departure = arrival.AddDays(1);
        }
        var roomType = match.Groups[4].Value;

        command = new AvailabilityCommand
        {
            HotelId = hotelId,
            Arrival = arrival,
            Departure = departure,
            RoomType = roomType
        };
        return true;
    }

    private static bool TryParseDateOnly(string s, out DateOnly date)
    {
        date = default;
        if (!DateTime.TryParseExact(s, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
        {
            return false;
        }
        date = DateOnly.FromDateTime(dt);
        return true;
    }
}
