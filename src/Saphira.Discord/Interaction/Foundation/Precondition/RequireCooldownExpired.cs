using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Core.Security.Cooldown;
using Saphira.Discord.Entity.Guild.Role;

namespace Saphira.Discord.Interaction.Foundation.Precondition;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RequireCooldownExpired(int cooldown) : PreconditionAttribute
{
    public override Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context,
        ICommandInfo commandInfo,
        IServiceProvider services
        )
    {
        var cooldownService = services.GetRequiredService<CooldownService>();
        var registryName = cooldownService.CreateCooldownRegistryName(commandInfo.Name);

        try
        {
            cooldownService.AddRegistry(registryName, TimeSpan.FromSeconds(cooldown), clearIfExists: false);
        }
        catch (ArgumentException)
        {
            // fine, registry already exists
        }

        if (context.User is not SocketGuildUser guildUser)
        {
            return Task.FromResult(PreconditionResult.FromError("User not found."));
        }

        // Team members have no cooldown
        if (guildUser.Roles.Any(role => GuildRole.IsTeamRole(role)))
        {
            return Task.FromResult(PreconditionResult.FromSuccess());
        }

        if (!cooldownService.CanUseActionAgain(registryName, guildUser, "usage"))
        {
            return Task.FromResult(PreconditionResult.FromError($"This command is on cooldown. Please wait a bit before using it again."));
        }

        return Task.FromResult(PreconditionResult.FromSuccess());
    }
}
