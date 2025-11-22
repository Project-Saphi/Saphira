using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Discord.Interaction.TypeConverter;
using System.Reflection;
using Saphira.Core.Application;
using Saphira.Discord.Logging;
using Saphira.Core.Security.Cooldown;

namespace Saphira.Discord.Interaction;

public class InteractionHandler(DiscordSocketClient client, InteractionService interactionService, IServiceProvider serviceProvider)
{
    private readonly Configuration _botConfiguration = serviceProvider.GetRequiredService<Configuration>();
    private readonly IMessageLogger _logger = serviceProvider.GetRequiredService<IMessageLogger>();
    private readonly CooldownService _cooldownService = serviceProvider.GetRequiredService<CooldownService>();

    public async Task InitializeAsync()
    {
        interactionService.AddTypeConverter<IEmote>(new EmoteTypeConverter());

        var assembly = Assembly.GetExecutingAssembly();
        _logger.Log(LogSeverity.Info, "Saphira", $"Loading interaction modules from {assembly.GetName().Name} ...");

        await interactionService.AddModulesAsync(assembly, serviceProvider);

        var totalModules = interactionService.Modules.Count;
        _logger.Log(LogSeverity.Info, "Saphira", $"Loaded {totalModules} interaction module(s)");

        client.InteractionCreated += HandleInteraction;
        client.Ready += RegisterCommandsAsync;

        interactionService.Log += LogAsync;
        interactionService.InteractionExecuted += HandleInteractionExecuted;
        interactionService.SlashCommandExecuted += HandleSlashCommandExecuted;
    }

    private async Task RegisterCommandsAsync()
    {
        await interactionService.RegisterCommandsToGuildAsync(_botConfiguration.GuildId);
        _logger.Log(LogSeverity.Info, "Saphira", $"Registered commands to guild {_botConfiguration.GuildId}");
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
                await RespondToInteractionAsync(interaction, $"An error occurred executing the command. See console log for more information.");
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

            // If a Precondition is not met we show the error to the user
            var userErrorMessage = "An error occurred while handling this interaction. See console log for more information.";

            if (result.Error == InteractionCommandError.UnmetPrecondition)
            {
                userErrorMessage = result.ErrorReason;
            }

            _logger.Log(LogSeverity.Warning, "Saphira", $"Command '{commandInfo.Name}' failed: {errorMessage}");

            try
            {
                await RespondToInteractionAsync(context.Interaction, userErrorMessage);
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

        var registryName = _cooldownService.CreateCooldownRegistryName(commandInfo.Name);
        _cooldownService.AddActionCooldown(registryName, guildUser, "usage", true);
    }

    private Task LogAsync(LogMessage log)
    {
        _logger.Log(log);
        return Task.CompletedTask;
    }
}
