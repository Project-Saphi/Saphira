using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Cronjobs;
using Saphira.Discord;
using Saphira.Discord.Guild;
using Saphira.Discord.Messaging;
using Saphira.Extensions.Caching;
using Saphira.Extensions.DependencyInjection;
using Saphira.Saphi.Api;
using Saphira.Util.Game;
using Saphira.Util.Game.Matchup;
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
        var botConfig = BuildBotConfiguration();

        if (!ValidateBotConfiguration(botConfig))
        {
            return;
        }

        _discordSocketClient = CreateClient();
        _interactionService = new InteractionService(_discordSocketClient.Rest);
        _serviceProvider = BuildServiceProvider(_discordSocketClient, _interactionService, botConfig);

        var interactionHandler = new InteractionHandler(_discordSocketClient, _interactionService, _serviceProvider);
        await interactionHandler.InitializeAsync();

        await _discordSocketClient.LoginAsync(TokenType.Bot, botConfig.BotToken);
        await _discordSocketClient.StartAsync();

        _serviceProvider.RegisterCronjobs();
        _serviceProvider.RegisterEventSubscribers();

        var cronjobScheduler = _serviceProvider.GetRequiredService<CronjobScheduler>();
        cronjobScheduler.ScheduleCronjobs();

        await Task.Delay(-1);
    }

    private static BotConfiguration BuildBotConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json", optional: false, reloadOnChange: true)
            .Build();

        return configuration.Get<BotConfiguration>() ?? throw new Exception("Failed to load configuration from config.json");
    }

    private static bool ValidateBotConfiguration(BotConfiguration botConfiguration)
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
        BotConfiguration botConfiguration
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
