using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Core.Logging;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;
using System.Reflection;

namespace Saphira.Discord.Interaction.SlashCommand;

public class HelpCommand(IServiceProvider serviceProvider) : BaseCommand
{
    private readonly IMessageLogger _logger = serviceProvider.GetRequiredService<IMessageLogger>();

    public override SlashCommandMetadata GetMetadata()
    {
        return new SlashCommandMetadata("/help");
    }

    [SlashCommand("help", "Receive a DM with all available commands")]
    public async Task HandleCommand()
    {
        var embedFields = new List<EmbedFieldBuilder>();

        var assembly = Assembly.GetExecutingAssembly();
        var commandTypes = assembly.GetTypes().Where(t => typeof(BaseCommand).IsAssignableFrom(t) && !t.IsAbstract && t != typeof(BaseCommand));

        foreach (var commandType in commandTypes)
        {
            if (ActivatorUtilities.CreateInstance(serviceProvider, commandType) is not BaseCommand commandInstance)
            {
                continue;
            }

            // We assume every class has only method that has the SlashCommand attribute
            // The module structure of Discord.NET allows multiple slash commands per module but I find this too messy
            // I prefer having one slash command per module :)
            var slashCommandAttribute = GetSlashCommandAttributes(commandType).FirstOrDefault();

            if (slashCommandAttribute == null)
            {
                continue;
            }

            var metadata = commandInstance.GetMetadata();
            embedFields.Add(CreateEmbedField(metadata, slashCommandAttribute));
        }

        var embed = new EmbedBuilder()
            .WithAuthor("Command Help")
            .WithTimestamp(DateTimeOffset.Now);

        foreach (var embedField in embedFields.OrderBy(f => f.Name))
        {
            embed.AddField(embedField);
        }

        try
        {
            await Context.User.SendMessageAsync(embed: embed.Build());
            await RespondAsync("You received the command help via DM.", ephemeral: true);
        }
        catch (Exception ex)
        {
            _logger.Log(LogSeverity.Error, "Saphira", $"Failed to DM the command help to {Context.User.GlobalName} ({Context.User.Id}): {ex}");
            await RespondAsync("Failed to DM the command help. Do you have DMs from server members enabled?", ephemeral: true);
        }
    }

    private EmbedFieldBuilder CreateEmbedField(SlashCommandMetadata metadata, SlashCommandAttribute slashCommandAttribute)
    {
        var embedField = new EmbedFieldBuilder()
            .WithName($"{MessageTextFormat.Bold($"/{slashCommandAttribute.Name}")}")
            .WithIsInline(true);

        var content = new List<string>()
        {
            slashCommandAttribute.Description
        };

        if (metadata.Example != null)
        {
            content.Add($"`{metadata.Example}`");
        }

        if (metadata.Notes != null)
        {
            content.Add("");
            content.Add($"{MessageTextFormat.Bold("Note")}: {metadata.Notes}");
        }

        embedField.WithValue(string.Join("\n", content));
        return embedField;
    }

    private List<SlashCommandAttribute> GetSlashCommandAttributes(Type commandType)
    {
        var slashCommandAttributes = new List<SlashCommandAttribute>();

        foreach (var method in commandType.GetMethods())
        {
            var slashCommandAttribute = method.GetCustomAttribute<SlashCommandAttribute>();

            if (slashCommandAttribute != null)
            {
                slashCommandAttributes.Add(slashCommandAttribute);
            }
        }

        return slashCommandAttributes;
    }
}