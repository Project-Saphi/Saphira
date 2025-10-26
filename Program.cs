using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Saphi.Api;
using Saphira.Util;

namespace Saphira
{
    public class Program
    {
        private static DiscordSocketClient _discordSocketClient = null!;
        private static InteractionService _interactionService = null!;
        private static IServiceProvider _serviceProvider = null!;

        public static DateTime StartTime { get; private set; }

        private static Task Log(string message)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }

        public static async Task Main()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                .Build();

            var config = configuration.Get<Configuration>() ?? throw new Exception("Failed to load configuration from config.json");

            if (string.IsNullOrWhiteSpace(config.BotToken))
            {
                Log("Error: Bot token is missing in config.json! Unable to start Saphira.");
                return;
            }

            var clientConfig = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true
            };

            _discordSocketClient = new DiscordSocketClient(clientConfig);
            _interactionService = new InteractionService(_discordSocketClient.Rest);

            _serviceProvider = new ServiceCollection()
                .AddSingleton(_discordSocketClient)
                .AddSingleton(_interactionService)
                .AddSingleton(config)
                .AddHttpClient()
                .AddSingleton<Client>()
                .AddSingleton<ScoreFormatter>()
                .BuildServiceProvider();

            var interactionHandler = new InteractionHandler(_discordSocketClient, _interactionService, _serviceProvider);
            await interactionHandler.InitializeAsync();

            await _discordSocketClient.LoginAsync(TokenType.Bot, config.BotToken);
            await _discordSocketClient.StartAsync();

            _discordSocketClient.Ready += () =>
            {
                StartTime = DateTime.UtcNow;

                Log("Connection to Discord established.");
                Log("Slash commands are being registered ...");
                Log("Saphira started successfully.");

                return Task.CompletedTask;
            };

            _discordSocketClient.Log += (message) =>
            {
                Console.WriteLine(message.ToString());
                return Task.CompletedTask;
            };

            await Task.Delay(-1);
        }
    }
}

