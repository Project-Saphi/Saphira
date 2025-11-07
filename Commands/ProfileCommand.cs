using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;

namespace Saphira.Commands;

[RequireTextChannel]
[RequireCommandAllowedChannel]
public class ProfileCommand : InteractionModuleBase<SocketInteractionContext>
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("profile", "See your user profile")]
    public async Task HandleCommand()
    {
        if (Context.User is not SocketGuildUser guildUser)
        {
            var errorAlert = new ErrorAlertEmbedBuilder("Your profile data cannot be fetched.");
            await RespondAsync(embed: errorAlert.Build());
            return;
        }

        var embed = new EmbedBuilder()
            .WithTimestamp(DateTimeOffset.Now)
            .WithThumbnailUrl(guildUser.GetDisplayAvatarUrl())
            .WithAuthor($"{guildUser.GlobalName}'s profile", guildUser.GetDisplayAvatarUrl());

        var footer = new EmbedFooterBuilder()
            .WithText($"ID: {guildUser.Id}");

        embed.WithFooter(footer);

        var fields = GetProfileEmbedFields(Context.User);
        foreach (var field in fields)
        {
            embed.AddField(field);
        }

        await RespondAsync(embed: embed.Build());
    }

    private List<EmbedFieldBuilder> GetProfileEmbedFields(SocketUser user)
    {
        var guildUser = user as SocketGuildUser;
        var embedFields = new List<EmbedFieldBuilder>();

        var profileField = new EmbedFieldBuilder()
            .WithName(":busts_in_silhouette: Profile");

        var profileFields = new[]
        {
            $"{MessageTextFormat.Bold("Joined")}: {guildUser?.JoinedAt:yyyy-MM-dd}",
            $"{MessageTextFormat.Bold("Registered")}: {user.CreatedAt:yyyy-MM-dd}"
        };

        profileField.WithValue(String.Join("\n", profileFields));
        embedFields.Add(profileField);

        return embedFields;
    }
}
