using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Core;
using Saphira.Core.Logging;
using Saphira.Core.Security.Cooldown;
using Saphira.Discord.Interaction.Foundation.TypeConverter;

namespace Saphira.Discord.Interaction;

public class InteractionHandler(IServiceProvider serviceProvider)
{
    private readonly Application _application = serviceProvider.GetRequiredService<Application>();
    private readonly InteractionService _interactionService = serviceProvider.GetRequiredService<InteractionService>();
    private readonly IMessageLogger _logger = serviceProvider.GetRequiredService<IMessageLogger>();
    private readonly CooldownService _cooldownService = serviceProvider.GetRequiredService<CooldownService>();

    public async Task InitializeAsync()
    {
        _interactionService.AddTypeConverter<IEmote>(new EmoteTypeConverter());

        foreach (var assembly in _application.LoadAssemblies())
        {
            _logger.Log(LogSeverity.Info, "Saphira", $"Loading interaction modules from {assembly.GetName().Name} ...");
            await _interactionService.AddModulesAsync(assembly, serviceProvider);
        }

        var totalModules = _interactionService.Modules.Count;
        _logger.Log(LogSeverity.Info, "Saphira", $"Loaded {totalModules} interaction module(s)");

        _interactionService.Log += LogAsync;
        _interactionService.InteractionExecuted += HandleInteractionExecuted;
        _interactionService.SlashCommandExecuted += HandleSlashCommandExecuted;
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

            // If a Precondition is not met we show the error to the user
            var userErrorMessage = "An error occurred while handling this interaction. See console log for more information.";

            if (result.Error == InteractionCommandError.UnmetPrecondition)
            {
                userErrorMessage = result.ErrorReason;
            }

            _logger.Log(LogSeverity.Warning, "Saphira", $"Command '{commandInfo.Name}' failed: {errorMessage}");

            try
            {
                await InteractionResponder.RespondAsync(context.Interaction, userErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.Log(LogSeverity.Error, "Saphira", $"Failed to send error response: {ex.Message}");
            }
        }
    }

    private async Task HandleSlashCommandExecuted(SlashCommandInfo commandInfo, IInteractionContext interactionContext, IResult result)
    {
        if (!result.IsSuccess || interactionContext.User is not SocketGuildUser guildUser)
        {
            return;
        }

        var hasCooldownAttribute = commandInfo.Module.Preconditions.Any(attr => attr is Foundation.Precondition.RequireCooldownExpired);

        if (hasCooldownAttribute)
        {
            var registryName = _cooldownService.CreateCooldownRegistryName(commandInfo.Name);
            _cooldownService.AddActionCooldown(registryName, guildUser, "usage", true);
        }
    }

    private async Task LogAsync(LogMessage log)
    {
        _logger.Log(log);
    }
}
