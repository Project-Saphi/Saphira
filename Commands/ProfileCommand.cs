using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;

namespace Saphira.Commands
{
    public class ProfileCommand : InteractionModuleBase<SocketInteractionContext>
    {
        [CommandContextType(InteractionContextType.Guild)]
        [RequireTextChannel]
        [SlashCommand("profile", "See your user profile")]
        public async Task HandleCommand()
        {
            if (Context.User is not SocketGuildUser guildUser)
            {
                // I don't think this should happen?
                // The user can't execute that command if he's not even a server member
                var errorAlert = new ErrorAlertEmbedBuilder("Your profile data cannot be fetched.");
                await RespondAsync(embed: errorAlert.Build());
                return;
            }

            var embed = new EmbedBuilder();
            embed.WithTimestamp(DateTimeOffset.UtcNow);
            embed.WithThumbnailUrl(guildUser.GetDisplayAvatarUrl());
            embed.WithAuthor($"{guildUser.GlobalName}'s profile", guildUser.GetDisplayAvatarUrl());

            var footer = new EmbedFooterBuilder();
            footer.WithText($"ID: {guildUser.Id}");

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

            var profileField = new EmbedFieldBuilder();
            profileField.WithName(":busts_in_silhouette: Profile");

            var profileFields = new[]
            {
                $"{MessageTextFormat.Bold("Joined")}: {guildUser?.JoinedAt.ToString().Substring(0, 10)}",
                $"{MessageTextFormat.Bold("Registered")}: {user.CreatedAt.ToString().Substring(0, 10)}"
            };

            profileField.WithValue(String.Join("\n", profileFields));
            embedFields.Add(profileField);

            return embedFields;
        }
    }
}
