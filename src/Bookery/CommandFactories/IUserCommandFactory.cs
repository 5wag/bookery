using System.Diagnostics.CodeAnalysis;
using Bookery.Commands;

namespace Bookery.CommandFactories;

public interface IUserCommandFactory
{
    bool TryCreate(string commandString, [NotNullWhen(true)] out IUserCommand? command);
}
