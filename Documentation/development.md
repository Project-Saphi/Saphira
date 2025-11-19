[<- Back](../README.md)

# Developer Documentation

## Table of Contents

- [Command Preconditions](#command-preconditions)
- [Autocompletion and ValueProviders](#autocompletion-and-valueproviders)
- [Pagination](#pagination)
- [Event Subscribers](#event-subscribers)
- [Cronjobs](#cronjobs)
- [Logging](#logging)
- [ASCII Tables](#ascii-tables)

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
namespace Saphira.Discord.Interaction.SlashCommand.Autocompletion.ValueProvider;

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
namespace Saphira.Discord.Interaction.SlashCommand;

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
namespace Saphira.Discord.Interaction.SlashCommand.Autocompletion.ValueProvider;

[AutoRegister]
public class PlayerValueProvider(CachedClient client) : IValueProvider
{
    // Other code ...
}
```

## Pagination

When a command returns a large dataset (such as a leaderboard, lists of tracks, or player statistics), pagination is a good way to reduce the amount of output that Saphira creates at once. The main idea behind pagination is to not run into Discord's message length limit.

To implement pagination, you need to pass the `PaginationComponentHandler` to the command you are creating. The `PaginationComponentHandler` is a singleton service which is responsible for storing the current state of the pagination and handling page transitions.

```csharp
public class LeaderboardCommand(CachedClient client, PaginationComponentHandler paginationComponentHandler) : BaseCommand
{
    // Other code ...
}
```

When responding to the command, you need to create the initial pagination state as well as create the pagination component itself:

```csharp
namespace Saphira.Discord.Interaction.SlashCommand;

public class LeaderboardCommand(CachedClient client, PaginationComponentHandler paginationComponentHandler) : BaseCommand
{
    [SlashCommand("leaderboard", "Get the leaderboard for a single track and category")]
    public async Task HandleCommand(
        [Autocomplete(typeof(CustomTrackAutocompleteHandler))] string track,
        [Autocomplete(typeof(CategoryAutocompleteHandler))] string category
        )
    {
        // Other code ...
        var leaderboardEntries = [...]; // Your data to paginate

        var initialPagination = new Pagination(1, 10, leaderboardEntries.Count);
        var paginationId = Guid.NewGuid();

        var paginationComponentBuilder = new PaginationComponentBuilder(
            paginationId,
            disablePrevious: initialPagination.IsFirstPage(),
            disableNext: initialPagination.IsLastPage()
        );       
    }
}
```

The `Pagination` is a simple object consisting of 3 parameters:

- `currentPage` - The current page number (1-indexed)
- `pageSize` - Number of items per page
- `itemCount` - Total number of items in the dataset

Based off of this, the `Pagination` can calculate the limit and offset, number of pages, previous and next page and many other things. The pagination buttons use custom IDs in the format `pagination:action:id`:

- `pagination:prev:{guid}` - Previous page button
- `pagination:next:{guid}` - Next page button

The `PaginationComponentBuilder` is a simple component builder which creates a "Previous Page" and a "Next Page" button which can be attached to the message you are sending. It requires a `Guid` to create the unique custom IDs for both buttons (this is important to identify each unique pagination).

After creating the pagination you can build your initial response:

```csharp
namespace Saphira.Discord.Interaction.SlashCommand;

public class LeaderboardCommand(CachedClient client, PaginationComponentHandler paginationComponentHandler) : BaseCommand
{
    [SlashCommand("leaderboard", "Get the leaderboard for a single track and category")]
    public async Task HandleCommand(
        [Autocomplete(typeof(CustomTrackAutocompleteHandler))] string track,
        [Autocomplete(typeof(CategoryAutocompleteHandler))] string category
        )
    {
        // Other code ...
        var leaderboardEntries = [...]; // Your data to paginate

        var initialPagination = new Pagination(1, 10, leaderboardEntries.Count);
        var paginationId = Guid.NewGuid();

        var paginationComponentBuilder = new PaginationComponentBuilder(
            paginationId,
            disablePrevious: initialPagination.IsFirstPage(),
            disableNext: initialPagination.IsLastPage()
        );
        
        var firstPageEntries = leaderboardEntries.Take(initialPagination.GetLimit()).ToList();
        var firstPageEmbed = GetEmbedForPage(customTrack, firstPageEntries, initialPagination.CurrentPage);

        await RespondAsync(embed: firstPageEmbed.Build(), components: paginationComponentBuilder.Build());
    }
}
```

After sending the response, you need to register the pagination inside the `PaginationComponentHandler`, along with a custom callback that handles page changes:

```csharp
namespace Saphira.Discord.Interaction.SlashCommand;

public class LeaderboardCommand(CachedClient client, PaginationComponentHandler paginationComponentHandler) : BaseCommand
{
    [SlashCommand("leaderboard", "Get the leaderboard for a single track and category")]
    public async Task HandleCommand(
        [Autocomplete(typeof(CustomTrackAutocompleteHandler))] string track,
        [Autocomplete(typeof(CategoryAutocompleteHandler))] string category
        )
    {
        var leaderboardEntries = [...]; // Your data to paginate

        var initialPagination = new Pagination(1, 10, leaderboardEntries.Count);
        var paginationId = Guid.NewGuid();

        var paginationComponentBuilder = new PaginationComponentBuilder(
            paginationId,
            disablePrevious: initialPagination.IsFirstPage(),
            disableNext: initialPagination.IsLastPage()
        );
        
        var firstPageEntries = leaderboardEntries.Take(initialPagination.GetLimit()).ToList();
        var firstPageEmbed = GetEmbedForPage(customTrack, firstPageEntries, initialPagination.CurrentPage);

        await RespondAsync(embed: firstPageEmbed.Build(), components: paginationComponentBuilder.Build());

        RegisterPagination(paginationId, initialPagination, leaderboardEntries, track);
    }

    private void RegisterPagination(
        Guid paginationId,
        Pagination pagination,
        List<TrackLeaderboardEntry> leaderboardEntries,
        CustomTrack track)
    {
        var state = new PaginationState(pagination, async (component, newPagination) =>
        {
            // Get entries for the new page
            var pageEntries = leaderboardEntries
                .Skip(newPagination.GetOffset())
                .Take(newPagination.GetLimit())
                .ToList();

            // Build the embed for the new page
            var embed = GetEmbedForPage(track, pageEntries, newPagination.CurrentPage);

            // Make sure you update the buttons as well to disable the previous / next page
            var updatedComponents = new PaginationComponentBuilder(
                paginationId,
                newPagination.IsFirstPage(),
                newPagination.IsLastPage()
            );

            // Update the message
            await component.UpdateAsync(msg =>
            {
                msg.Embed = embed.Build();
                msg.Components = updatedComponents.Build();
            });
        });

        paginationComponentHandler.RegisterPagination(paginationId, state);
    }
}
```

Pagination states are automatically cleaned up after 5 minutes of inactivity to prevent memory accumulation. When a user tries to interact with an expired pagination, they'll see an "This pagination has expired" message.

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

## ASCII Tables

It's 2025 and yet Discord still provides no way to display content as a table. While embeds support up to 3 columns they don't have a very nice mobile layout and are also limited by the maximum amount of characters in embed fields.

That's where ASCII tables come into play: Free from some of the restrictions of embeds, ASCII tables can be a nice alternative to displaying tables. The `AsciiTableBuilder` is a utility class to build such tables and render them. It automatically handles column width calculations, text alignment, and wrapping content in code blocks for proper monospace formatting.

### Basic Usage

```csharp
using Saphira.Discord.Messaging;

var table = new AsciiTableBuilder()
    .AddHeader("Track", "Player", "Time")
    .AddRow("Crash Cove", "Garma", "1:15.23")
    .AddRow("Roo's Tubes", "Niikasd", "1:42.89")
    .AddRow("Tiger Temple", "RedHot", "1:38.45")
    .Build();

await ReplyAsync(table);
```

This produces:

```
| Track        | Player  | Time    |
|--------------|---------|---------|
| Crash Cove   | Garma   | 1:15.23 |
| Roo's Tubes  | Niikasd | 1:42.89 |
| Tiger Temple | RedHot  | 1:38.45 |
```

### Available Methods

#### AddHeader(params string[] headers)

Adds column headers to the table. This should be called before adding rows.

```csharp
.AddHeader("Column1", "Column2", "Column3")
```

#### AddRow(params object[] cells)

Adds a data row to the table. Accepts any type of object (will be converted to string). If fewer cells are provided than there are headers, the remaining cells will be empty.

```csharp
.AddRow("Value1", "Value2", "Value3")
.AddRow(123, 456.78, true)  // Works with any object type
```

#### SetColumnAlignment(int columnIndex, ColumnAlignment alignment)

Sets the text alignment for a specific column. Available alignments: `Left`, `Right`, `Center`.

```csharp
.SetColumnAlignment(0, AsciiTableBuilder.ColumnAlignment.Left)
.SetColumnAlignment(1, AsciiTableBuilder.ColumnAlignment.Right)
.SetColumnAlignment(2, AsciiTableBuilder.ColumnAlignment.Center)
```

#### SetColumnSeparator(char separator)

Changes the column separator character (default: `|`).

```csharp
.SetColumnSeparator('│')  // Unicode box drawing character
```

#### SetHeaderSeparator(char separator)

Changes the header separator character (default: `-`).

```csharp
.SetHeaderSeparator('═')  // Unicode box drawing character
```

#### SetWrapInCodeBlock(bool wrap)

Controls whether the table should be wrapped in Discord code blocks (default: `true`). When enabled, the table is wrapped in triple backticks for proper monospace formatting.

```csharp
.SetWrapInCodeBlock(false)  // Don't wrap in code block
```

There is generally no reason to disable this, as rendering without monospace formatting will likely mess up the layout of the table.

#### Build()

Builds and returns the final table as a string.
