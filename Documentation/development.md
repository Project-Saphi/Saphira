[<- Back](../README.md)

# Developer Documentation

## Table of Contents

- [Command Preconditions](#command-preconditions)
- [Autocompletion and ValueProviders](#autocompletion-and-valueproviders)
- [Event Subscribers](#event-subscribers)
- [Cronjobs](#cronjobs)
- [Logging](#logging)

## Command Preconditions

There are 3 command preconditions that I find useful when developing new commands:

- `RequireTeamMemberRole`
- `RequireTextChannel`
- `RequireCommandAllowedChannel`

Instead of binding commands to permissions, I often bind commands to roles. Saphira, for example, has a bunch of moderation commands that are generally available for all team members. That's what the `RequireTeamMemberRole` precondition does - it checks for the `Saphi Team` role and restricts commands to users with that role.

Most commands I develop are text-based and don't make any sense for non-text channels, such as voice channels or event channels. That's why I added the `RequireTextChannel` precondition, as this allows you to restrict a command to text channels. And since you usually want to restrict commands to certain text channels, there is also the `RequireCommandAllowedChannel` precondition.

## Autocompletion and ValueProviders

[Discord.NET](https://docs.discordnet.dev/index.html) allows building custom autocompletion handlers, but by default the existing design pattern causes you to write very repetitive and similar code, especially if you have object lists that have a very similar structure. That's why I have added the concept of a `ValueProvider` on top, which allows the fast creation of basic autocompletion handlers.

Using a `ValueProvider` is really simple. First, you need an entity:

```csharp
namespace Saphira.Saphi.Entity;

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
namespace Saphira.Commands.Autocompletion.ValueProvider;

public class PlayerValueProvider(CachedClient client) : IValueProvider
{
    public async Task<List<Value>> GetValuesAsync()
    {
        var players = await client.GetPlayersAsync(); // We assume GetPlayersAsync() returns a List<Player>
        return [.. players.Select(p => new Value(int.Parse(p.Id), p.Name))];
    }
}
```

Now that you have the `PlayerValueProvider`, you can create a new `PlayerAutocompleteHandler`:

```csharp
public class PlayerAutocompleteHandler : BasicAutocompleteHandler<PlayerValueProvider> { }
```

You don't even need to implement anything because the `BasicAutocompleteHandler` already does all the logic for you! Now you can specify the `PlayerAutocompleteHandler` when creating a new command:

```csharp
namespace Saphira.Commands;

public class PBsCommand() : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("pbs", "Get personal best times of a player")]
    public async Task HandleCommand([Autocomplete(typeof(PlayerAutocompleteHandler))] string player)
    {
        // Other code ...
    }
}
```

To be able to use a `ValueProvider` as a dependency, you can attach the `AutoRegister` attribute to it. That way, the `ValueProvider` is automatically added to the service collection when Saphira is started:

```csharp
using Saphira.Extensions.DependencyInjection;

namespace Saphira.Commands.Autocompletion.ValueProvider;

[AutoRegister]
public class PlayerValueProvider(CachedClient client) : IValueProvider
{
    // Other code ...
}
```

## Event Subscribers

Since [Discord.NET](https://docs.discordnet.dev/index.html)'s way of subscribing to events is very barebones, I decided to wrap a thin layer around the event system. Basically, it allows you to move event subscribers to a separate class and auto-register them, so you don't need to take care of instantiating the class and manually registering the event subscriber.

First, create your event subscriber:

```csharp
namespace Saphira.Discord.EventSubscriber;

public class CustomReadyEventSubscriber(DiscordSocketClient client) : IDiscordSocketClientEventSubscriber
{

    private bool _isRegistered = false;

    public void Register()
    {
        if (_isRegistered) return;

        client.Ready += HandleReadyAsync;
        _isRegistered = true;
    }

    public void Unregister()
    {
        if (!_isRegistered) return;

        client.Ready -= HandleReadyAsync;
        _isRegistered = false;
    }

    private Task HandleReadyAsync()
    {
        Console.WriteLine("Bot is ready");
        return Task.CompletedTask;
    }
}
```

Event subscribers need to implement the `IDiscordSocketClientEventSubscriber` interface. This interface requires 2 methods `Register()` and `Unregister()`, although `Unregister()` is currently not used. From there, you can just subscribe to any event from the client that you want and extend with a custom method - in this example the `HandleReadyAsync()` method.

After creating your event subscriber, you can attach the custom `AutoRegister` attribute to it:

```csharp
namespace Saphira.Discord.EventSubscriber;

[AutoRegister]
public class CustomReadyEventSubscriber : IDiscordSocketClientEventSubscriber
{
    // Other code ...
}
```

And that's it! The event subscriber will now automatically be loaded into the service collection.

## Cronjobs

I am going to preface this section by saying: What I implemented aren't cronjobs, even though I call them that. They are tasks which are scheduled at a certain interval. I call them cronjobs because I feel the word "cronjob" is associated more with any kind of time-based execution these days.

Creating a new cronjob is super easy:

```csharp
namespace Saphira.Cronjobs;

[AutoRegister]
public class TestCronjob : ICronjob
{
    public Task ExecuteAsync()
    {
        Console.WriteLine("Running test cronjob!")
        return Task.CompletedTask;
    }

    public TimeSpan GetStartDelay()
    {
        return GetInterval();
    }

    public TimeSpan GetInterval()
    {
        return TimeSpan.FromMinutes(30);
    }
}
```

Cronjobs need to implement the `ICronjob` interface, which requires 3 methods to be implemented:

- `ExecuteAsync()` contains the execution logic of the cronjob
- `GetStartDelay()` must return a `TimeSpan` which defines how long the first execution is delayed after the bot is started
- `GetInterval()` must return a `TimeSpan` which defines how long the next execution is delayed

When creating a cronjob, you can attach the `AutoRegister` attribute to it, so that the cronjob is automatically registered in the `CronjobScheduler`.

## Logging

There is a `ConsoleMessageLogger` utility, which allows logging messages at different severities and in different colors to the console. Using the `ConsoleMessageLogger` is really simple:

```csharp
namespace Saphira.Discord.EventSubscriber;

public class ReadyEventSubscriber(IMessageLogger logger) : IDiscordSocketClientEventSubscriber
{
    private bool _isRegistered = false;

    private Task HandleReadyAsync()
    {
        logger.Log(LogSeverity.Info, "Saphira", "Connection to Discord established.");
        logger.Log(LogSeverity.Info, "Saphira", "Saphira started successfully.");

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