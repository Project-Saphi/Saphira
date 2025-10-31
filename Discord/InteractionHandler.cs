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
            _interactionService.InteractionExecuted += HandleInteractionExecuted;
        }

        private async Task RegisterCommandsAsync()
        {
            await _interactionService.RegisterCommandsToGuildAsync(_configuration.GuildId);
            _logger.Log(LogSeverity.Info, "Saphira", $"Registered commands to guild {_configuration.GuildId}");
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
                    if (interaction.HasResponded)
                    {
                        await interaction.FollowupAsync($"An error occurred: {ex.Message}", ephemeral: true);
                    }
                    else
                    {
                        await interaction.RespondAsync($"An error occurred: {ex.Message}", ephemeral: true);
                    }
                }
            }
        }

        private async Task HandleInteractionExecuted(ICommandInfo commandInfo, IInteractionContext context, IResult result)
        {
            if (!result.IsSuccess)
            {
                var errorMessage = result.Error switch
                {
                    InteractionCommandError.UnmetPrecondition => result.ErrorReason,
                    InteractionCommandError.UnknownCommand => "Unknown command.",
                    InteractionCommandError.BadArgs => "Invalid arguments provided.",
                    InteractionCommandError.Exception => $"Command exception: {result.ErrorReason}",
                    InteractionCommandError.Unsuccessful => "Command could not be executed.",
                    _ => "An unknown error occurred."
                };

                _logger.Log(LogSeverity.Warning, "Saphira", $"Command '{commandInfo.Name}' failed: {errorMessage}");

                try
                {
                    if (context.Interaction.HasResponded)
                    {
                        await context.Interaction.FollowupAsync(errorMessage, ephemeral: true);
                    }
                    else
                    {
                        await context.Interaction.RespondAsync(errorMessage, ephemeral: true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log(LogSeverity.Error, "Saphira", $"Failed to send error response: {ex.Message}");
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
