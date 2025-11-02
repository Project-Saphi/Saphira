using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Commands.Autocompletion.ValueProvider;
using Saphira.Cronjobs;
using Saphira.Discord;
using Saphira.Discord.Guild;
using Saphira.Discord.Messaging;
using Saphira.Extensions.Caching;
using Saphira.Extensions.DependencyInjection;
using Saphira.Saphi.Api;
using Saphira.Util.Game;
using Saphira.Util.Logging;

namespace Saphira;

public class Program
{
    private static DiscordSocketClient _discordSocketClient = null!;
    private static InteractionService _interactionService = null!;
    private static IServiceProvider _serviceProvider = null!;

    public static DateTime StartTime { get; set; }

    public static async Task Main()
    {
        var config = BuildConfiguration();

        if (string.IsNullOrWhiteSpace(config.BotToken))
        {
            Console.WriteLine("Bot token is missing in config.json. Unable to start Saphira.");
            return;
        }

        _discordSocketClient = CreateClient();
        _interactionService = new InteractionService(_discordSocketClient.Rest);
        _serviceProvider = BuildServiceProvider(_discordSocketClient, _interactionService, config);

        var interactionHandler = new InteractionHandler(_discordSocketClient, _interactionService, _serviceProvider);
        await interactionHandler.InitializeAsync();

        await _discordSocketClient.LoginAsync(TokenType.Bot, config.BotToken);
        await _discordSocketClient.StartAsync();

        _serviceProvider.RegisterCronjobs();
        _serviceProvider.RegisterEventSubscribers();

        var cronjobScheduler = _serviceProvider.GetRequiredService<CronjobScheduler>();
        cronjobScheduler.ScheduleCronjobs();

        await Task.Delay(-1);
    }

    private static Configuration BuildConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json", optional: false, reloadOnChange: true)
            .Build();

        return configuration.Get<Configuration>() ?? throw new Exception("Failed to load configuration from config.json");
    }

    private static DiscordSocketClient CreateClient()
    {
        var clientConfig = new DiscordSocketConfig
        {
            AlwaysDownloadUsers = true,
            AuditLogCacheSize = 0,
            GatewayIntents = GatewayIntents.All,
            LogGatewayIntentWarnings = false,
            MessageCacheSize = 100
        };

        return new DiscordSocketClient(clientConfig);
    }

    private static IServiceProvider BuildServiceProvider(
        DiscordSocketClient discordSocketClient,
        InteractionService interactionService,
        Configuration configuration
        )
    {
        return new ServiceCollection()
            .AddSingleton(discordSocketClient)
            .AddSingleton(interactionService)
            .AddSingleton(configuration)
            .AddSingleton<GuildRoleManager>()
            .AddSingleton<CacheInvalidationService>()
            .AddSingleton<CachedClient>()
            .AddSingleton<IMessageLogger, ConsoleMessageLogger>()
            .AddSingleton<CronjobScheduler>()
            .AddTransient<CustomTrackValueProvider>()
            .AddTransient<CategoryValueProvider>()
            .AddTransient<CharacterValueProvider>()
            .AddTransient<PlayerValueProvider>()
            .AddTransient<ToggleableRoleValueProvider>()
            .AddTransient<GuildManager>()
            .AddTransient<ScoreFormatter>()
            .AddTransient<InviteLinkDetector>()
            .AddTransient<RestrictedContentDetector>()
            .AddHttpClient()
            .AddMemoryCache()
            .AddCronjobs()
            .AddEventSubscribers()
            .BuildServiceProvider();
    }
}
