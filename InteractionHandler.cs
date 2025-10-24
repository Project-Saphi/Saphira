using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Saphira
{
    public class InteractionHandler
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly InteractionService _interactionService;
        private readonly IServiceProvider _serviceProvider;
        private readonly Configuration _configuration;

        public InteractionHandler(DiscordSocketClient client, InteractionService interactionService, IServiceProvider services)
        {
            _discordSocketClient = client;
            _interactionService = interactionService;
            _serviceProvider = services;
            _configuration = services.GetRequiredService<Configuration>();
        }

        public async Task InitializeAsync()
        {
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

            _discordSocketClient.InteractionCreated += HandleInteraction;
            _discordSocketClient.Ready += RegisterCommandsAsync;
            _interactionService.Log += LogAsync;
        }

        private async Task RegisterCommandsAsync()
        {
            Console.WriteLine($"Registering commands to guild {_configuration.GuildId} (instant) ...");
            await _interactionService.RegisterCommandsToGuildAsync(_configuration.GuildId);
        }

        private async Task HandleInteraction(SocketInteraction interaction)
        {
            try
            {
                var context = new SocketInteractionContext(_discordSocketClient, interaction);
                await _interactionService.ExecuteCommandAsync(context, _serviceProvider);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling interaction: {ex.Message}");
                if (interaction.Type == InteractionType.ApplicationCommand)
                {
                    await interaction.RespondAsync($"An error occurred: {ex.Message}", ephemeral: true);
                }
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }
    }
}
