[<- Back](../README.md)

# Developer Documentation

## Command Preconditions

There are 2 command preconditions that I find useful when developing new commands:

- `RequireTeamMemberRole`
- `RequireTextChannel`

Instead of binding commands to permissions, I often bind commands to roles. Saphira, for example, has a bunch of moderation commands that are generally available for all team members. That's what the `RequireTeamMemberRole` precondition does - it checks for the `Saphi Team` role and restricts commands to users with that role.

Most commands I develop are text-based and don't make any sense for non-text channels, such as voice channels or event channels. That's why I added the `RequireTextChannel` precondition, as this allows you to restrict a command to text channels.

## Autocompletion and ValueProviders

[Discord.NET](https://docs.discordnet.dev/index.html) allows building custom autocompletion handlers, but by default the existing design pattern causes you to write very repetitive and similar code, especially if you have object lists that have a very similar structure. That's why I have added the concept of a `ValueProvider` on top, which allows the fast creation of basic autocompletion handlers.

Using a `ValueProvider` is really simple. First, you need an entity:

```csharp
public class Player
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}
```

The entity can come from anywhere - in this example it's a REST entity from an external API, so it also contains JSON decoding instructions. Next, you can implement the actual `ValueProvider`.

```csharp
namespace Saphira.Commands.Autocompletion.ValueProvider
{
    public class PlayerValueProvider : IValueProvider
    {
        private readonly Client _client;
        private readonly IMemoryCache _cache;

        public PlayerValueProvider(Client client)
        {
            _client = client;
        }

        public async Task<List<Value>> GetValuesAsync()
        {
            var values = new List<Value>();
            var players = await _client.GetPlayersAsync(); // We assume GetPlayersAsync() returns a List<Player>

            foreach (var player in players)
            {
                var value = new Value(int.Parse(player.Id), player.Name);
                values.Add(value);
            }

            return values;
        }
    }
}
```

Now that you have the `PlayerValueProvider`, you can create a new `PlayerAutocompleteHandler`:

```csharp
public class PlayerAutocompleteHandler : BasicAutocompleteHandler<PlayerValueProvider> { }
```

You don't even need to implement anything because the `BasicAutocompleteHandler` already does all the logic for you! Now you can specify the `PlayerAutocompleteHandler` when creating a new command:

```csharp
[SlashCommand("pbs", "Get personal best times of a player")]
public async Task HandleCommand([Autocomplete(typeof(PlayerAutocompleteHandler))] string player)
{
    // Other code ...
}
```

## Event Subscribers

Since [Discord.NET](https://docs.discordnet.dev/index.html)'s way of subscribing to events is very barebones, I decided to wrap a thin layer around the event system. Basically, it allows you to move event subscribers to a separate class and auto-register them, so you don't need to take care of instantiating the class and manually registering the event subscriber.

First, create your event subscriber:

```csharp
namespace Saphira.Discord.EventSubscriber
{
    public class CustomReadyEventSubscriber : IDiscordSocketClientEventSubscriber
    {
        private readonly DiscordSocketClient _client;

        private bool _isRegistered = false;

        public CustomReadyEventSubscriber(DiscordSocketClient client)
        {
            _client = client;
        }

        public void Register()
        {
            if (_isRegistered) return;

            _client.Ready += HandleReadyAsync;
            _isRegistered = true;
        }

        public void Unregister()
        {
            if (!_isRegistered) return;

            _client.Ready -= HandleReadyAsync;
            _isRegistered = false;
        }

        private Task HandleReadyAsync()
        {
            Console.WriteLine("Bot is ready");
            return Task.CompletedTask;
        }
    }
}
```

Event subscribers need to implement the `IDiscordSocketClientEventSubscriber` interface. This interface requires 2 methods `Register()` and `Unregister()`, although `Unregister()` is currently not used. From there, you can just subscribe to any event from the client that you want and extend with a custom method - in this example the `HandleReadyAsync()` method.

After creating your event subscriber, you can attach the custom `AutoRegister` attribute to it:

```csharp
namespace Saphira.Discord.EventSubscriber
{
    [AutoRegister]
    public class CustomReadyEventSubscriber : IDiscordSocketClientEventSubscriber
    {
        // Other code ...
    }
}
```

And that's it! The event subscriber will now automatically be loaded into the service collection.

## Logging

There is a `ConsoleMessageLogger` utility, which allows logging messages at different severities and in different colors to the console. Using the `ConsoleMessageLogger` is really simple:

```csharp
public class ReadyEventSubscriber : IDiscordSocketClientEventSubscriber
{
    private readonly IMessageLogger _logger;
    private bool _isRegistered = false;

    public ReadyEventSubscriber(IMessageLogger logger)
    {
        _logger = logger;
    }

    private Task HandleReadyAsync()
    {
        _logger.Log(LogSeverity.Info, "Saphira", "Connection to Discord established.");
        _logger.Log(LogSeverity.Info, "Saphira", "Saphira started successfully.");

        return Task.CompletedTask;
    }
}
```

The `IMessageLogger` here is an instance of the `ConsoleMessageLogger`. It has only a single method `Log`, with 4 different parameters:

- `severity` - An instance of `Discord.LogSeverity` which specifies the severity level
- `source`  - A string which contains the source of the log entry - can be any arbitrary string
- `message` - The message that should be logged - can be any arbitrary string
- `exception` - An instance of `System.Exception` which contains an exception (optional)

Depending on the severity, different colors are used:

| Severity | Color     |
|----------|-----------|
| Critical | Dark Red  |
| Error    | Red       |
| Warning  | Yellow    |
| Info     | White     |
| Verbose  | Gray      |
| Debug    | Dark Gray |

Messages written to the console using the `ConsoleMessageLogger` will look like this:

```
[12:03:12] [Gateway] Connecting
[12:03:13] [Gateway] Connected
[12:03:13] [Saphira] Registering commands to guild 1080098783184040006 ...
[12:03:13] [Saphira] Connection to Discord established.
[12:03:13] [Saphira] Saphira started successfully.
[12:03:13] [Gateway] Ready
```

All messages contain the current timestamp, the source, and the message.