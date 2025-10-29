using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Commands.Autocompletion.TypeConverter;
using Saphira.Util.Logging;
using System.Reflection;

namespace Saphira.Discord
{
    public class InteractionHandler
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly InteractionService _interactionService;
        private readonly IServiceProvider _serviceProvider;
        private readonly Configuration _configuration;
        private readonly IMessageLogger _logger;

        public InteractionHandler(DiscordSocketClient client, InteractionService interactionService, IServiceProvider serviceProvider)
        {
            _discordSocketClient = client;
            _interactionService = interactionService;
            _serviceProvider = serviceProvider;
            _configuration = serviceProvider.GetRequiredService<Configuration>();
            _logger = serviceProvider.GetRequiredService<IMessageLogger>();
        }

        public async Task InitializeAsync()
        {
            _interactionService.AddTypeConverter<IEmote>(new EmoteTypeConverter());

            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

            _discordSocketClient.InteractionCreated += HandleInteraction;
            _discordSocketClient.Ready += RegisterCommandsAsync;
            _interactionService.Log += LogAsync;
        }

        private async Task RegisterCommandsAsync()
        {
            _logger.Log(LogSeverity.Info, "Saphira", $"Registering commands to guild {_configuration.GuildId} ...");
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
                _logger.Log(LogSeverity.Error, "Saphira", $"Error handling interaction: {ex.Message}");
                if (interaction.Type == InteractionType.ApplicationCommand)
                {
                    await interaction.RespondAsync($"An error occurred: {ex.Message}", ephemeral: true);
                }
            }
        }

        private Task LogAsync(LogMessage log)
        {
            _logger.Log(log);
            return Task.CompletedTask;
        }
    }
}
