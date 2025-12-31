using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Core;
using Saphira.Core.Cronjob;
using Saphira.Core.Extensions.Caching;
using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Core.Logging;
using Saphira.Core.Security.Cooldown;
using Saphira.Core.Twitch;
using Saphira.Discord.Core.Entity.Guild;
using Saphira.Discord.Core.Extensions.DependencyInjection;
using Saphira.Discord.Core.Interaction;
using Saphira.Discord.Messaging;
using Saphira.Discord.Pagination.Component;
using Saphira.Saphi.Api;

namespace Saphira;

public class Program
{
    public static async Task Main()
    {
        var configuration = BuildConfiguration();

        if (!ValidateConfiguration(configuration))
        {
            return;
        }

        var client = CreateClient();
        var interactionService = new InteractionService(client.Rest);
        var serviceProvider= BuildServiceProvider(client, interactionService, configuration);

        var interactionHandler = new InteractionHandler(serviceProvider);
        await interactionHandler.InitializeAsync();

        await client.LoginAsync(TokenType.Bot, configuration.BotToken);
        await client.StartAsync();

        serviceProvider.RegisterCronjobs();
        serviceProvider.RegisterEventSubscribers();

        var cronjobScheduler = serviceProvider.GetRequiredService<CronjobScheduler>();
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

    private static bool ValidateConfiguration(Configuration configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration.BotToken))
        {
            Console.WriteLine("Bot token is missing in config.json. Unable to start Saphira.");
            return false;
        }

        if (configuration.MaxAutocompleteSuggestions < 1 || configuration.MaxAutocompleteSuggestions > 25)
        {
            Console.WriteLine("The number of autocomplete suggestions must be between 1 and 25.");
            return false;
        }

        if (!configuration.SaphiApiBaseUrl.StartsWith("https"))
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
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers | GatewayIntents.GuildMessages | GatewayIntents.MessageContent | GatewayIntents.GuildPresences | GatewayIntents.DirectMessages,
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
            .AddSingleton<CacheInvalidationService>()
            .AddSingleton<ISaphiApiClient, SaphiApiClient>()
            .AddSingleton<IMessageLogger, ConsoleMessageLogger>()
            .AddSingleton<CronjobScheduler>()
            .AddSingleton<PaginationComponentHandler>()
            .AddSingleton<CooldownService>()
            .AddSingleton<Application>()
            .AddTransient<GuildManager>()
            .AddTransient<InviteLinkDetector>()
            .AddTransient<RestrictedContentDetector>()
            .AddTransient<TwitchClient>()
            .AddHttpClient()
            .AddMemoryCache()
            .AddCronjobs()
            .AddEventSubscribers()
            .AddValueProviders()
            .BuildServiceProvider();
    }
}
