using Bookery;
using Bookery.CommandFactories;
using Bookery.Commands;
using Bookery.Data;
using CommandLine;

Parser.Default.ParseArguments<CommandLineArgs>(args)
    .WithParsed(RunProgram)
    .WithNotParsed(HandleCommandLineArgsParsingError);

static void RunProgram(CommandLineArgs cmd)
{
    var deserializer = new Deserializer();
    var bookings = deserializer.Deserialize<Booking[]>(File.ReadAllText(cmd.BookingsFilePath));
    var hotels = deserializer.Deserialize<Hotel[]>(File.ReadAllText(cmd.HotelsFilePath));
    if (hotels is null || bookings is null)
    {
        Console.WriteLine("Could not read data files.");
        Environment.Exit(1);
    }

    var data = new DataStore 
    { 
        Hotels = [.. hotels], 
        Bookings = [.. bookings]
    };

    List<IUserCommandFactory> commandFactories = [
        new AvailabilityCommandFactory(), 
        new SearchCommandFactory()
        ];

    while (true)
    {
        Console.WriteLine("Enter command:");
        var line = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(line))
        {
            Environment.Exit(0);
        }

        IUserCommand? command = null;
        foreach (var factory in commandFactories)
        {
            if (factory.TryCreate(line, out command))
            {
                var result = command.Execute(data);
                Console.WriteLine(result);
                break;
            }
        }
        
        if (command is null)
        {
            Console.WriteLine("Unknown command.");
            continue;
        }
    }
}

static void HandleCommandLineArgsParsingError(IEnumerable<Error> _)
{
    Console.WriteLine("Please provide valid command line arguments.");
}