using Bookery.Commands;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Bookery.CommandFactories;

public class SearchCommandFactory : IUserCommandFactory
{
    /// <example>
    /// Search(H1, 365, SGL)
    /// </example>
    private readonly Regex _commandRegex = new Regex(@"^Search\(\s*([a-zA-Z\d]+)\s*,\s*(\d+)\s*,\s*([a-zA-Z\d]+\s*)\)$");

    public bool TryCreate(string commandString, [NotNullWhen(true)] out IUserCommand? command)
    {
        command = null;
        
        var match = _commandRegex.Match(commandString);
        if (!match.Success)
        {
            return false;
        }

        var days = int.Parse(match.Groups[2].Value);
        if (days == 0)
        {
            return false;
        }

        command = new SearchCommand
        {
            HotelId = match.Groups[1].Value,
            From = DateOnly.FromDateTime(DateTime.UtcNow),
            Days = int.Parse(match.Groups[2].Value),
            RoomType = match.Groups[3].Value
        };

        return true;
    }
}
