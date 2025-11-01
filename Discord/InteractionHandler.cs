using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Commands.Autocompletion.TypeConverter;
using Saphira.Util.Logging;
using System.Reflection;

namespace Saphira.Discord
{
    public class InteractionHandler(DiscordSocketClient client, InteractionService interactionService, IServiceProvider serviceProvider)
    {
        private readonly Configuration _configuration = serviceProvider.GetRequiredService<Configuration>();
        private readonly IMessageLogger _logger = serviceProvider.GetRequiredService<IMessageLogger>();

        public async Task InitializeAsync()
        {
            interactionService.AddTypeConverter<IEmote>(new EmoteTypeConverter());

            await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);

            client.InteractionCreated += HandleInteraction;
            client.Ready += RegisterCommandsAsync;
            interactionService.Log += LogAsync;
            interactionService.InteractionExecuted += HandleInteractionExecuted;
        }

        private async Task RegisterCommandsAsync()
        {
            await interactionService.RegisterCommandsToGuildAsync(_configuration.GuildId);
            _logger.Log(LogSeverity.Info, "Saphira", $"Registered commands to guild {_configuration.GuildId}");
        }

        private async Task RespondToInteractionAsync(IDiscordInteraction interaction, string message)
        {
            if (interaction.HasResponded)
            {
                await interaction.FollowupAsync(message, ephemeral: true);
            }
            else
            {
                await interaction.RespondAsync(message, ephemeral: true);
            }
        }

        private async Task HandleInteraction(SocketInteraction interaction)
        {
            try
            {
                var context = new SocketInteractionContext(client, interaction);
                await interactionService.ExecuteCommandAsync(context, serviceProvider);
            }
            catch (Exception ex)
            {
                _logger.Log(LogSeverity.Error, "Saphira", $"Error handling interaction: {ex.Message}");

                if (interaction.Type == InteractionType.ApplicationCommand)
                {
                    await RespondToInteractionAsync(interaction, $"An error occurred: {ex.Message}");
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
                    await RespondToInteractionAsync(context.Interaction, errorMessage);
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
