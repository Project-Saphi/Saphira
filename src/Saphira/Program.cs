using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Core.Application;
using Saphira.Core.Extensions.Caching;
using Saphira.Discord.Cronjob;
using Saphira.Discord.Extensions.DependencyInjection;
using Saphira.Discord.Guild;
using Saphira.Discord.Interaction;
using Saphira.Discord.Logging;
using Saphira.Discord.Messaging;
using Saphira.Discord.Messaging.Pagination;
using Saphira.Saphi.Api;
using Saphira.Util.Game;
using Saphira.Util.Game.Matchup;

namespace Saphira;

public class Program
{
    private static DiscordSocketClient _discordSocketClient = null!;
    private static InteractionService _interactionService = null!;
    private static IServiceProvider _serviceProvider = null!;

    public static DateTime StartTime { get; set; }

    public static async Task Main()
    {
        var configuration = BuildConfiguration();

        if (!ValidateBotConfiguration(configuration))
        {
            return;
        }

        _discordSocketClient = CreateClient();
        _interactionService = new InteractionService(_discordSocketClient.Rest);
        _serviceProvider = BuildServiceProvider(_discordSocketClient, _interactionService, configuration);

        var interactionHandler = new InteractionHandler(_discordSocketClient, _interactionService, _serviceProvider);
        await interactionHandler.InitializeAsync();

        await _discordSocketClient.LoginAsync(TokenType.Bot, configuration.BotToken);
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

    private static bool ValidateBotConfiguration(Configuration botConfiguration)
    {
        if (string.IsNullOrWhiteSpace(botConfiguration.BotToken))
        {
            Console.WriteLine("Bot token is missing in config.json. Unable to start Saphira.");
            return false;
        }

        if (botConfiguration.MaxAutocompleteSuggestions < 1 || botConfiguration.MaxAutocompleteSuggestions > 25)
        {
            Console.WriteLine("The number of autocomplete suggestions must be between 1 and 25.");
            return false;
        }

        if (!botConfiguration.SaphiApiBaseUrl.StartsWith("https"))
        {
            Console.WriteLine("Connection to the Saphi API can only be established via HTTPS.");
            return false;
        }

        return true;
    }

    private static DiscordSocketClient CreateClient()
    {
        var clientConfig = new DiscordSocketConfig
        {
            AuditLogCacheSize = 0,
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers | GatewayIntents.GuildMessages | GatewayIntents.MessageContent,
            MessageCacheSize = 100
        };

        return new DiscordSocketClient(clientConfig);
    }

    private static IServiceProvider BuildServiceProvider(
        DiscordSocketClient discordSocketClient,
        InteractionService interactionService,
        Configuration botConfiguration
        )
    {
        return new ServiceCollection()
            .AddSingleton(discordSocketClient)
            .AddSingleton(interactionService)
            .AddSingleton(botConfiguration)
            .AddSingleton<GuildRoleManager>()
            .AddSingleton<CacheInvalidationService>()
            .AddSingleton<CachedClient>()
            .AddSingleton<IMessageLogger, ConsoleMessageLogger>()
            .AddSingleton<CronjobScheduler>()
            .AddSingleton<PaginationComponentHandler>()
            .AddTransient<GuildManager>()
            .AddTransient<ScoreFormatter>()
            .AddTransient<InviteLinkDetector>()
            .AddTransient<RestrictedContentDetector>()
            .AddTransient<PlayerMatchupCalculator>()
            .AddHttpClient()
            .AddMemoryCache()
            .AddCronjobs()
            .AddEventSubscribers()
            .AddValueProviders()
            .BuildServiceProvider();
    }
}
