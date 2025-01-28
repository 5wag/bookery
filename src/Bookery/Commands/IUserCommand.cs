using Bookery.Data;

namespace Bookery.Commands;

public interface IUserCommand
{
    string Execute(DataStore data);
}
