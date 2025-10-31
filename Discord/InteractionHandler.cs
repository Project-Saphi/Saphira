using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Commands.Autocompletion.TypeConverter;
using Saphira.Discord.Guild;
using Saphira.Discord.Messaging;
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
            await _interactionService.RegisterCommandsToGuildAsync(_configuration.GuildId);
            _logger.Log(LogSeverity.Info, "Saphira", $"Registered commands to guild {_configuration.GuildId}");
        }

        private async Task HandleInteraction(SocketInteraction interaction)
        {
            try
            {
                var context = new SocketInteractionContext(_discordSocketClient, interaction);

                if (CanExecuteCommand(context.User, context.Channel))
                {
                    await _interactionService.ExecuteCommandAsync(context, _serviceProvider);
                }
                else
                {
                    if (interaction.Type == InteractionType.ApplicationCommand)
                    {
                        var warningAlert = new WarningAlertEmbedBuilder("You cannot use commands in this channel.");
                        await interaction.RespondAsync(embed: warningAlert.Build(), ephemeral: true);
                    }
                }
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

        private bool CanExecuteCommand(SocketUser user, ISocketMessageChannel channel)
        {
            var guildUser = user as SocketGuildUser;
            var guildChannel = channel as SocketGuildChannel;

            if (guildUser == null || guildChannel == null)
            {
                return false;
            }

            if (_configuration.CommandsAllowedChannels.Any(c => c == guildChannel.Name) || GuildMember.IsTeamMember(user))
            {
                return true;
            }

            return false;
        }

        private Task LogAsync(LogMessage log)
        {
            _logger.Log(log);
            return Task.CompletedTask;
        }
    }
}
